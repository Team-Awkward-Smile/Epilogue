﻿using Epilogue.Actors.Hestmor.aim;
using Epilogue.Extensions;
using Epilogue.Global.Singletons;
using Epilogue.Nodes;
using Epilogue.Props;
using Godot;
using System.Data;
using System.Linq;

namespace Epilogue.Actors.Hestmor;
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
	private NpcEvents _npcEvents;

	/// <inheritdoc/>
	public override void _Ready()
	{
		_aim = Owner.GetChildren().OfType<Aim>().FirstOrDefault();

		if (_aim is null)
		{
			GD.PrintErr($"Aim Node not present in Actor [{Owner.Name}]. Node Gun will not work properly");
		}

		_pickupArea = GetNode<Area2D>("PickupRange");

		if (_pickupArea is null)
		{
			GD.PrintErr($"Pickup Area not defined for Actor [{Owner.Name}]. Guns won't be able to be picked up");
		}

		_aim.AimAngleUpdated += UpdateGunRotation;

		_aimingArm = GetNode<Node2D>("AimingArm");
		_gunAnchor = (Node2D)_aimingArm.GetChild(0);
		_gunEvents = GetNode<GunEvents>("/root/GunEvents");
		_playerEvents = GetNode<PlayerEvents>("/root/PlayerEvents");
		_npcEvents = GetNode<NpcEvents>("/root/NpcEvents");

		_npcEvents.Connect(NpcEvents.SignalName.GunAcquiredFromNpc, Callable.From((Gun gun) => EquipGun(gun)));
		_playerEvents.Connect(PlayerEvents.SignalName.PlayerIsDying, Callable.From(DropGun), (uint)ConnectFlags.OneShot);
		
	}

	/// <summary>
	///		Method called whenever an input is detected to press/release the trigger of the equipped gun 
	/// </summary>
	/// <param name="triggerIsPressed">Is the input pressed or released?</param>
	/// <returns><c>true</c>, if the gun had ammo to operate when the trigger is activated; <c>false</c>, otherwise</returns>
	public bool InteractWithTrigger(bool triggerIsPressed)
	{
		if (triggerIsPressed && _currentGun.CurrentAmmoCount == 0)
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
		if (HasGunEquipped)
		{
			if (_currentGun is not Sword)
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
		var gun = (Gun)_pickupArea.GetOverlappingAreas()[0].Owner;

		gun.GetParent().RemoveChild(gun);

		EquipGun(gun);
	}

	private void EquipGun(Gun gun)
	{
		gun.Freeze = true;
		gun.RotationDegrees = 0f;

		_gunAnchor.AddChild(gun);

		gun.Position = new Vector2(0f, 0f);

		_gunEvents.EmitSignal(GunEvents.SignalName.PlayerPickedUpGun, gun.CurrentAmmoCount, gun.MaxAmmoCount);

		if (gun is Sword)
		{
			((Player)Owner).HoldingSword = true;
		}

		_currentGun = gun;
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
		if (_currentGun is null)
		{
			return;
		}

		InteractWithTrigger(false);

		var oldGun = UnequipGun();

		if (oldGun.CurrentAmmoCount == 0)
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

		if (keepRotation)
		{
			_currentGun.Rotation = _aimingArm.Rotation;
		}

		_currentGun.GlobalPosition = _gunAnchor.GlobalPosition;
		_currentGun.Freeze = false;

		_currentGun = null;

		_gunEvents.EmitSignal(GunEvents.SignalName.GunWasDropped);

		return gun;
	}

	private void UpdateGunRotation(int angleDegrees)
	{
		_aimingArm.RotationDegrees = angleDegrees;
	}


	/// The try is for if its not from gun class, so it doesn't crash
	private void _on_pickup_range_area_entered(Node2D area) {
		try
		{

			var item = (Gun)area.GetParent();
			if (item is Gun && item != _currentGun)
			{
				item.Highlighted = true;	
			}
		}
		catch (System.Exception)
		{
			
			throw;
		}
		
	}

	private void _on_pickup_range_area_exited(Node2D area){
		try
		{
			var item = (Gun)area.GetParent();
			if (item is Gun)
			{
				item.Highlighted = false;	
			}
		}
		catch (System.Exception)
		{
			
			throw;
		}
	}
}
