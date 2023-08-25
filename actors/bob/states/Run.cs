using Epilogue.global.enums;
using Epilogue.nodes;
using Godot;

namespace Epilogue.actors.hestmor.states;
public partial class Run : PlayerState
{
	[Export] private float _runSpeed = 200f;

	private bool _runToggled;

	public override void OnInput(InputEvent @event)
	{
		if(Input.IsActionJustPressed("jump"))
		{
			if(Actor.IsOnWall())
			{
				if(Actor.RayCasts["Head"].IsColliding())
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
		else if(Input.IsActionJustPressed("melee"))
		{
			StateMachine.ChangeState("MeleeAttack");
		}
		else if(Input.IsActionJustPressed("slide"))
		{
			StateMachine.ChangeState("Slide");
		}
		else if(Input.IsActionJustPressed("toggle_run"))
		{
			_runToggled = !_runToggled;
		}
	}

	public override void OnEnter()
	{
		_runToggled = true;

		// TODO: Replace with a proper run animation
		AnimPlayer.Play("walk", -1, 2f);

		Actor.CanChangeFacingDirection = true;
	}

	public override void PhysicsUpdate(double delta)
	{
		var movementDirection = Input.GetAxis("move_left", "move_right");

		if(movementDirection != 0f)
		{
			var velocity = Actor.Velocity;

			velocity.Y += Gravity * (float) delta;
			velocity.X = movementDirection * _runSpeed;

			if((movementDirection > 0 && Actor.FacingDirection == ActorFacingDirectionEnum.Left) ||
				(movementDirection < 0 && Actor.FacingDirection == ActorFacingDirectionEnum.Right))
			{
				velocity.X /= 2;
			}

			Actor.Velocity = velocity;
		}

		Actor.MoveAndSlideWithRotation();

		if(movementDirection == 0f || Actor.IsOnWall())
		{
			StateMachine.ChangeState("Idle");
		}
		else if(!Actor.IsOnFloor())
		{
			StateMachine.ChangeState("Fall");
		}
		else if(!_runToggled)
		{
			StateMachine.ChangeState("Walk");
		}
	}
}
