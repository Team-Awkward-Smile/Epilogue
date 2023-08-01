using Epilogue.actors.hestmor.aim;
using Epilogue.global.enums;
using Epilogue.global.singletons;
using Epilogue.guns;
using Epilogue.nodes;
using Epilogue.util;
using Godot;
using System.Linq;

namespace Epilogue.actors.hestmor;
public partial class GunSystem : Node2D
{
	public bool HasGunEquipped => _currentGun is not null;

	private Aim _aim;
	private Area2D _pickupArea;
	private Actor _actor;
	private Gun _currentGun;
	private Node2D _aimingArm;
	private Node2D _gunAnchor;
	private Events _events;

	public override void _Input(InputEvent @event)
	{
		// If this Node detects this Input, it will be handled and won't reach the rest of the Nodes
		if(@event.IsAction(InputUtils.GetInputActionName("pickup_or_drop_gun")) && @event.IsPressed())
		{
			if(HasGunEquipped)
			{
				DropGun();
				GetViewport().SetInputAsHandled();
			}
			else if(_actor.StateMachine.CanInteract && _pickupArea.GetOverlappingAreas().Any())
			{
				PickUpGun();
				GetViewport().SetInputAsHandled();
			}
		}
		else if(@event.IsAction(InputUtils.GetInputActionName("shoot")))
		{
			// KNOWN: 68 - Attacking with LMB without a gun equipped usually drops the input and do nothing
			if(HasGunEquipped)
			{
				if(_currentGun.CurrentAmmoCount > 0)
				{
					_currentGun.TriggerIsPressed = @event.IsPressed();
				}
				else
				{
					DropGun();
				}

				GetViewport().SetInputAsHandled();
			}
		}
	}

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
		_events = GetNode<Events>("/root/Events");
	}

	private void PickUpGun()
	{
		_currentGun = (Gun) _pickupArea.GetOverlappingAreas().First().Owner;
		_currentGun.GetParent().RemoveChild(_currentGun);
		_currentGun.Freeze = true;
		_currentGun.RotationDegrees = 0f;

		_gunAnchor.AddChild(_currentGun);

		_currentGun.Position = new Vector2(0f, 0f);

		_events.EmitGlobalSignal("PlayerPickedUpGun", _currentGun.CurrentAmmoCount, _currentGun.MaxAmmoCount);
	}

	private void DropGun()
	{
		_currentGun.GetParent().RemoveChild(_currentGun);

		GetTree().Root.AddChild(_currentGun);

		_currentGun.GlobalPosition = _gunAnchor.GlobalPosition;
		_currentGun.Freeze = false;
		_currentGun.ApplyImpulse(new Vector2(_actor.FacingDirection == ActorFacingDirectionEnum.Left ? 200f : -200f, -100f));

		_currentGun = null;

		_events.EmitGlobalSignal("GunWasDropped");
	}

	private void UpdateGunRotation(int angleDegrees)
	{
		_aimingArm.RotationDegrees = angleDegrees;
	}
}
