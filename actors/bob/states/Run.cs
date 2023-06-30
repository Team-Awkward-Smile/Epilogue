using Epilogue.extensions;
using Epilogue.nodes;
using Godot;
using System;

namespace Epilogue.actors.hestmor.states;
public partial class Run : StateComponent
{
	[Export] private float _runSpeed = 200f;

	public override void OnInput(InputEvent @event)
	{
		if(Input.IsActionJustPressed(_jumpInput))
		{
			if(Actor.IsOnWall())
			{
				if(Actor.IsHeadRayCastColliding())
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
		else if(Input.IsActionJustPressed(_attackInput))
		{
			StateMachine.ChangeState("MeleeAttack");
		}
		else if(Input.IsActionJustPressed(_slideInput))
		{
			StateMachine.ChangeState("Slide");
		}
	}

	public override void OnEnter()
	{
		AnimPlayer.Play("Bob/Walking", -1, 2f);
	}

	public override void PhysicsUpdate(double delta)
	{
		var movementDirection = Input.GetAxis(_moveLeftInput, _moveRightInput);

		if(movementDirection != 0f)
		{
			var rotationContainer = Actor.GetRotationContainer();
			rotationContainer.Scale = new Vector2(movementDirection < 0f ? -1 : 1, 1f);

			var velocity = Actor.Velocity;

			velocity.Y += Gravity * (float) delta;
			velocity.X = movementDirection * _runSpeed;

			Actor.Velocity = velocity;
		}

		Actor.MoveAndSlide();

		if(movementDirection == 0f || Actor.IsOnWall())
		{
			StateMachine.ChangeState("Idle");
		}
		else if(!Actor.IsOnFloor())
		{
			StateMachine.ChangeState("Fall");
		}
		else if(!Input.IsActionPressed(_toggleRunInput) && Math.Abs(movementDirection) < 0.5f)
		{
			StateMachine.ChangeState("Walk");
		}
	}
}
