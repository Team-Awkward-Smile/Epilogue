using Epilogue.global.enums;
using Epilogue.nodes;
using Godot;

namespace Epilogue.actors.hestmor.states;
/// <summary>
///		State that allows Hestmor to walk
/// </summary>
public partial class Walk : PlayerState
{
	[Export] private float _walkSpeed = 100f;

	internal override void OnInput(InputEvent @event)
	{
		if(@event.IsActionPressed(JumpInput))
		{
			StateMachine.ChangeState("Jump");
		}
		else if(@event.IsActionPressed(CrouchInput))
		{
			StateMachine.ChangeState("Crouch");
		}
		else if(@event.IsActionPressed(SlideInput))
		{
			StateMachine.ChangeState("Slide");
		}
		else if(@event.IsActionPressed(MeleeAttackInput))
		{
			StateMachine.ChangeState("MeleeAttack");
		}
	}

	internal override void OnEnter()
	{
		AnimPlayer.Play("walk");

		Player.CanChangeFacingDirection = true;
	}

	internal override void PhysicsUpdate(double delta)
	{
		var movementDirection = Input.GetAxis(MoveLeftDigitalInput, MoveRightDigitalInput);

		if(movementDirection == 0f)
		{
			// KNOWN: 68 - The analog movement works even in Retro Mode
			movementDirection = Input.GetAxis(MoveLeftAnalogInput, MoveRightAnalogInput);
		}

		if(movementDirection != 0f)
		{
			var velocity = Player.Velocity;

			velocity.Y += Gravity * (float) delta;
			velocity.X = movementDirection * _walkSpeed * (float) delta * 60f;

			if(movementDirection > 0 && Player.FacingDirection == ActorFacingDirection.Left ||
				movementDirection < 0 && Player.FacingDirection == ActorFacingDirection.Right)
			{
				velocity.X /= 2;
			}

			Player.Velocity = velocity;
		}

		Player.MoveAndSlideWithRotation();

		if(movementDirection == 0f || Player.IsOnWall())
		{
			StateMachine.ChangeState("Idle");
		}
		else if(!Player.IsOnFloor())
		{
			StateMachine.ChangeState("Fall");
		}
		else if(Player.RunEnabled)
		{
			StateMachine.ChangeState("Run");
		}
	}
}
