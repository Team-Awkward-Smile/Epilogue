using Epilogue.actors.hestmor.aim;
using Epilogue.global.enums;
using Epilogue.global.singletons;
using Epilogue.nodes;
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
	private Actor _actor;
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

		_actor = (Actor) Owner;

		_aim.AimAngleUpdated += UpdateGunRotation;

		_aimingArm = GetNode<Node2D>("AimingArm");
		_gunAnchor = (Node2D) _aimingArm.GetChild(0);
		_gunEvents = GetNode<GunEvents>("/root/GunEvents");
		_playerEvents = GetNode<PlayerEvents>("/root/PlayerEvents");
	}

	/// <summary>
	///		Method called whenever an input is detected to fire an equipped gun. If the gun is empty, it will be dropped instead
	/// </summary>
	/// <param name="isTriggerPressed">Is the input pressed or released?</param>
	public void TryFireGun(bool isTriggerPressed)
	{
		if(HasGunEquipped)
		{
			if(_currentGun.CurrentAmmoCount > 0)
			{
				_currentGun.TriggerIsPressed = isTriggerPressed;
			}
			else
			{
				DropGun();
			}
		}
	}

	/// <summary>
	///		Method called whenever an input is detected to interact with a gun (either picking it up or dropping it)
	/// </summary>
	public void InteractWithGun()
	{
		if(HasGunEquipped)
		{
			DropGun();
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
	}

	private void DropGun()
	{
		_currentGun.GetParent().RemoveChild(_currentGun);

		GetTree().Root.AddChild(_currentGun);

		_currentGun.GlobalPosition = _gunAnchor.GlobalPosition;
		_currentGun.Freeze = false;
		_currentGun.ApplyImpulse(new Vector2(_actor.FacingDirection == ActorFacingDirection.Left ? 200f : -200f, -100f));

		if(_currentGun.CurrentAmmoCount == 0)
		{
			_currentGun.SelfDestruct();
		}

		_currentGun = null;

		_gunEvents.EmitGlobalSignal("GunWasDropped");
	}

	private void UpdateGunRotation(int angleDegrees)
	{
		_aimingArm.RotationDegrees = angleDegrees;
	}
}
