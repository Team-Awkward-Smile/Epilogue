using Epilogue.actors.hestmor.aim;
using Epilogue.global.enums;
using Epilogue.nodes;
using Epilogue.util;
using Godot;
using System.Linq;

namespace Epilogue.actors.hestmor;
public partial class GunSystem : Node2D
{
	public bool HasGunEquipped { get; private set; }

	private Aim _aim;
	private Area2D _pickupArea;
	private Actor _actor;

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
		else if(@event.IsAction(InputUtils.GetInputActionName("shoot")) && @event.IsPressed())
		{
			if(HasGunEquipped)
			{
				GD.Print("Shooting");

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
	}

	private void PickUpGun()
	{
		HasGunEquipped = true;
		_pickupArea.GetOverlappingAreas().First().Owner.QueueFree();
	}

	private void DropGun()
	{
		HasGunEquipped = false;

		var gun = GD.Load<PackedScene>("res://temp/temp_gun.tscn").Instantiate() as RigidBody2D;

		GetTree().Root.AddChild(gun);

		gun.GlobalPosition = GlobalPosition - new Vector2(0f, 20f);

		gun.ApplyImpulse(new Vector2(_actor.FacingDirection == ActorFacingDirectionEnum.Left ? 200f : -200f, -100f));
	}
}
