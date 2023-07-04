using Epilogue.global.enums;
using Epilogue.global.singletons;
using Epilogue.nodes;
using Godot;

namespace Epilogue.actors.hestmor.states;
public partial class Walk : StateComponent
{
	[Export] private float _walkSpeed = 100f;

	public override void OnInput(InputEvent @event)
	{
		if(@event.IsAction(Settings.GetActionName("jump")))
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
					//StateMachine.ChangeState("GrabLedge");
				}
			}
			else
			{
				StateMachine.ChangeState("Jump");
			}
		}
		else if(@event.IsAction(Settings.GetActionName("melee")))
		{
			StateMachine.ChangeState("MeleeAttack");
		}
		else if(@event.IsAction(Settings.GetActionName("crouch")))
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
		var movementDirection = Input.GetAxis(_moveLeftInput, _moveRightInput);

		if(movementDirection != 0f)
		{
			Actor.SetFacingDirection(movementDirection < 0 ? ActorFacingDirection.Left : ActorFacingDirection.Right);

			var velocity = Actor.Velocity;

			velocity.Y += Gravity * (float) delta;
			velocity.X = movementDirection * _walkSpeed * (float) delta * 60f;

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
		//else if(true)
		//{
		//	StateMachine.ChangeState("Run");
		//}
	}
}
