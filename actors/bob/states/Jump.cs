using Epilogue.global.enums;
using Epilogue.nodes;
using Godot;

namespace Epilogue.actors.hestmor.states;
/// <summary>
///		State that allows Hestmor to start a jump
/// </summary>
public partial class Jump : PlayerState
{
	[Export] private float _jumpSpeed = -400f;

	private float _horizontalVelocity;

	private void StartJump(StringName animName)
	{
		AnimPlayer.AnimationFinished -= StartJump;
		Player.Velocity = new Vector2(_horizontalVelocity, _jumpSpeed);
	}

	internal override void OnEnter()
	{
		if(Player.Velocity.X == 0)
		{
			_horizontalVelocity = 100f * (Player.FacingDirection == ActorFacingDirection.Left ? -1 : 1);
		}
		else
		{
			_horizontalVelocity = 100f * (Player.Velocity.X > 0 ? 1 : -1);
		}

		AudioPlayer.PlayGenericSfx("Jump");

		Player.Velocity = new Vector2(0f, Player.Velocity.Y);
		Player.CanChangeFacingDirection = false;

		AnimPlayer.Play("jump");
		AnimPlayer.AnimationFinished += StartJump;
	}

	internal override void PhysicsUpdate(double delta)
	{
		Player.Velocity = new Vector2(Player.Velocity.X, Player.Velocity.Y + (Gravity * (float) delta));
		Player.MoveAndSlideWithRotation();

		if(Player.Velocity.Y > 0)
		{
			StateMachine.ChangeState("Fall");
			return;
		}
		else if(Player.IsOnFloor() && Player.Velocity.Y < 0)
		{
			StateMachine.ChangeState("Idle");
		}
		else if(Player.RayCasts["Head"].IsColliding() && !Player.RayCasts["Ledge"].IsColliding())
		{
			StateMachine.ChangeState("GrabLedge");
		}
	}
}
