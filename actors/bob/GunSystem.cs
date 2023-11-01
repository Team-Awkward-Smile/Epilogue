using Epilogue.actors.hestmor.aim;
using Epilogue.global.enums;
using Epilogue.global.singletons;
using Epilogue.nodes;
using Epilogue.props;
using Epilogue.util;

using Godot;

using System.Linq;

namespace Epilogue.actors.hestmor;
/// <summary>
///		Node responsible for the gun currently equipped by Hestmor. Constrols shooting, picking and dropping, etc.
/// </summary>
public partial class GunSystem : Node2D
{
	/// <summary>
	///		Defines if Hestmor is currently holding a gun or not
	/// </summary>
	public bool HasGunEquipped => _currentGun is not null;

	/// <summary>
	///		Defines if there are any nearby guns that player can interact with
	/// </summary>
    public bool IsAnyGunNearby => _pickupArea.GetOverlappingAreas().Any();

    private Aim _aim;
	private Area2D _pickupArea;
	private Gun _currentGun;
	private Node2D _aimingArm;
	private Node2D _gunAnchor;
	private GunEvents _gunEvents;
	private PlayerEvents _playerEvents;

	/// <inheritdoc/>
	public override void _Ready()
	{
		_aim = Owner.GetChildren().OfType<Aim>().FirstOrDefault();

		if(_aim is null)
		{
			GD.PrintErr($"Aim Node not present in Actor [{Owner.Name}]. Node Gun will not work properly");
		}

		_pickupArea = GetNode<Area2D>("PickupRange");

		if(_pickupArea is null)
		{
			GD.PrintErr($"Pickup Area not defined for Actor [{Owner.Name}]. Guns won't be able to be picked up");
		}

		_aim.AimAngleUpdated += UpdateGunRotation;

		_aimingArm = GetNode<Node2D>("AimingArm");
		_gunAnchor = (Node2D) _aimingArm.GetChild(0);
		_gunEvents = GetNode<GunEvents>("/root/GunEvents");
		_playerEvents = GetNode<PlayerEvents>("/root/PlayerEvents");
	}

	/// <summary>
	///		Method called whenever an input is detected to press/release the trigger of the equipped gun 
	/// </summary>
	/// <param name="triggerIsPressed">Is the input pressed or released?</param>
	/// <returns><c>true</c>, if the gun had ammo to operate when the trigger is activated; <c>false</c>, otherwise</returns>
	public bool InteractWithTrigger(bool triggerIsPressed)
	{
		if(triggerIsPressed && _currentGun.CurrentAmmoCount == 0)
		{
			return false;
		}

		_currentGun.TriggerIsPressed = triggerIsPressed;

		return true;
	}

	/// <summary>
	///		Method called whenever an input is detected to interact with a gun (either picking it up or dropping it)
	/// </summary>
	public void InteractWithGun()
	{
		if(HasGunEquipped)
		{
			if(_currentGun is not Sword)
			{
				DropGun();
			}
		}
		else
		{
			PickUpGun();
		}
	}

	private void PickUpGun()
	{
		_currentGun = (Gun) _pickupArea.GetOverlappingAreas().First().Owner;
		_currentGun.GetParent().RemoveChild(_currentGun);
		_currentGun.Freeze = true;
		_currentGun.RotationDegrees = 0f;

		_gunAnchor.AddChild(_currentGun);

		_currentGun.Position = new Vector2(0f, 0f);

		_gunEvents.EmitGlobalSignal("PlayerPickedUpGun", _currentGun.CurrentAmmoCount, _currentGun.MaxAmmoCount);

		if(_currentGun is Sword)
		{
			((Player) Owner).HoldingSword = true;
		}
	}

	/// <summary>
	///		Throws the equipped gun in the direction the player is aiming at
	/// </summary>
	public void ThrowGun()
	{
		var oldGun = UnequipGun(true);

		oldGun.TransformIntoProjectile();
	}

	/// <summary>
	///		Drops the equipped gun at the player's current position, without adding any impulse or altering it's properties
	/// </summary>
	private void DropGun()
	{
		InteractWithTrigger(false);

		var oldGun = UnequipGun();

		if(oldGun.CurrentAmmoCount == 0)
		{
			oldGun.SelfDestruct();
		}
	}

	/// <summary>
	///		Unequips the current gun by removing it from the Player scene and adding it to the Tree as an independent Node
	/// </summary>
	/// <returns>A reference to the unequipped gun to be used for further interactions</returns>
	private Gun UnequipGun(bool keepRotation = true)
	{
		var gun = _currentGun;

		_currentGun.GetParent().RemoveChild(_currentGun);

		GetTree().GetLevel().AddChild(_currentGun);

		if(keepRotation)
		{
			_currentGun.Rotation = _aimingArm.Rotation;
		}

		_currentGun.GlobalPosition = _gunAnchor.GlobalPosition;
		_currentGun.Freeze = false;
		_currentGun = null;

		_gunEvents.EmitGlobalSignal("GunWasDropped");

		return gun;
	}

	private void UpdateGunRotation(int angleDegrees)
	{
		_aimingArm.RotationDegrees = angleDegrees;
	}
}
