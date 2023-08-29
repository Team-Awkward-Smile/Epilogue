using Epilogue.constants;
using Epilogue.global.enums;
using Epilogue.nodes;
using Godot;

using Microsoft.CodeAnalysis.Operations;

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
		if(Player.IsOnWall() && Player.SweepForLedge(out var ledgePosition))
		{
			var offset = Player.RayCasts["Head"].GlobalPosition.Y - ledgePosition.Y;

			if(offset < -30)
			{
				Player.Position = new Vector2(Player.Position.X, ledgePosition.Y + Constants.MAP_TILE_SIZE);
				StateMachine.ChangeState("Vault");
			}
			else
			{
				Player.Position -= new Vector2(0f, offset);
				StateMachine.ChangeState("GrabLedge");
			}

			return;
		}

		Player.Velocity = new Vector2(Player.Velocity.X, Player.Velocity.Y + (Gravity * (float) delta));
		Player.MoveAndSlideWithRotation();

		if(Player.Velocity.Y > 0)
		{
			StateMachine.ChangeState("Fall");
		}
		else if(Player.IsOnFloor() && Player.Velocity.Y < 0)
		{
			StateMachine.ChangeState("Idle");
		}
	}
}
