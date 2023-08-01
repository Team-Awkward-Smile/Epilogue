using Epilogue.global.enums;
using Epilogue.nodes;
using Godot;

namespace Epilogue.actors.hestmor.states;
public partial class Walk : StateComponent
{
	[Export] private float _walkSpeed = 100f;

	/// <inheritdoc/>
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
		else if(Input.IsActionJustPressed(_slideInput))
		{
			StateMachine.ChangeState("Slide");
		}
	}

	/// <inheritdoc/>
	public override void OnEnter()
	{
		AnimPlayer.Play("walk");

		Actor.CanChangeFacingDirection = true;
	}

	/// <inheritdoc/>
	public override void PhysicsUpdate(double delta)
	{
		var movementDirection = Input.GetAxis(_moveLeftDigitalInput, _moveRightDigitalInput);

		if(movementDirection == 0f)
		{
			// KNOWN: 68 - The analog movement works even in Retro Mode
			movementDirection = Input.GetAxis(_moveLeftAnalogInput, _moveRightAnalogInput);
		}

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
		else if(Player.MovementInputManager.RunEnabled)
		{
			StateMachine.ChangeState("Run");
		}
	}
}
