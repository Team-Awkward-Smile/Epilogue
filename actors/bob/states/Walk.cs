using Epilogue.extensions;
using Epilogue.nodes;
using Godot;
using System;

namespace Epilogue.actors.hestmor.states;
public partial class Walk : StateComponent
{
	[Export] private float _walkSpeed = 100f;

	public override void OnInput(InputEvent @event)
	{
		if(Input.IsActionJustPressed("jump"))
		{
			if(Character.IsOnWall())
			{
				if(Character.IsHeadRayCastColliding())
				{
					// Is near a wall
					StateMachine.ChangeState("Jump");
				}
				else
				{
					// Is near a ledge
					StateMachine.ChangeState("GrabLedge");
				}
			}
			else
			{
				StateMachine.ChangeState("Jump");
			}
		}
		else if(Input.IsActionJustPressed("attack"))
		{
			StateMachine.ChangeState("MeleeAttack");
		}
		else if(Input.IsActionJustPressed("crouch"))
		{
			StateMachine.ChangeState("Crouch");
		}
	}

	public override void OnEnter()
	{
		AnimPlayer.Play("Bob/Walking");
	}

	public override void PhysicsUpdate(double delta)
	{
		var movementDirection = Input.GetAxis("move_left", "move_right");

		if(movementDirection != 0f)
		{
			var rotationContainer = Character.GetRotationContainer();
			rotationContainer.Scale = new Vector2(movementDirection < 0f ? -1 : 1, 1f);

			var velocity = Character.Velocity;

			velocity.Y += Gravity * (float) delta;
			velocity.X = movementDirection * _walkSpeed * (float) delta * 100;

			Character.Velocity = velocity;
		}

		Character.MoveAndSlide();

		if(movementDirection == 0f || Character.IsOnWall())
		{
			StateMachine.ChangeState("Idle");
		}
		else if(!Character.IsOnFloor())
		{
			StateMachine.ChangeState("Fall");
		}
		else if(!Input.IsActionPressed("toggle_walk") && Math.Abs(movementDirection) >= 0.5f)
		{
			StateMachine.ChangeState("Run");
		}
	}
}
