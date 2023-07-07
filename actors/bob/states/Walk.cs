using Epilogue.global.enums;
using Epilogue.nodes;
using Godot;

namespace Epilogue.actors.hestmor.states;
public partial class Walk : StateComponent
{
	[Export] private float _walkSpeed = 100f;

	private bool _runToggled = false;

	public override void OnInput(InputEvent @event)
	{
		if(Input.IsActionJustPressed(_jumpInput))
		{
			if(Actor.RayCasts["Head"].IsColliding() && !Actor.RayCasts["Ledge"].IsColliding())
			{
				StateMachine.ChangeState("GrabLedge");
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
		else if(Input.IsActionJustPressed(_crouchInput))
		{
			StateMachine.ChangeState("Crouch");
		}
		else if(Input.IsActionJustPressed(_toggleRunInput))
		{
			_runToggled = !_runToggled;
		}
		else if(Input.IsActionJustPressed(_slideInput))
		{
			StateMachine.ChangeState("Slide");
		}
	}

	public override void OnEnter()
	{
		_runToggled = false;

		AnimPlayer.Play("walk");

		Actor.CanChangeFacingDirection = true;
	}

	public override void PhysicsUpdate(double delta)
	{
		var movementDirection = Input.GetAxis(_moveLeftInput, _moveRightInput);

		if(movementDirection != 0f)
		{
			var velocity = Actor.Velocity;

			velocity.Y += Gravity * (float) delta;
			velocity.X = movementDirection * _walkSpeed * (float) delta * 60f;

			if(movementDirection > 0 && Actor.FacingDirection == ActorFacingDirectionEnum.Left ||
				movementDirection < 0 && Actor.FacingDirection == ActorFacingDirectionEnum.Right)
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
		else if(_runToggled)
		{
			StateMachine.ChangeState("Run");
		}
	}
}
