using Epilogue.actors.hestmor.enums;
using Epilogue.constants;
using Epilogue.global.enums;
using Epilogue.global.singletons;
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
	[Export] private float _lowJumpHorizontalSpeed = 80f;
	[Export] private float _longJumpHorizontalSpeed = 160f;

	private float _horizontalVelocity;
	private Achievements _achievements;
	private JumpData _jumpData;

	public override void _Ready()
	{
		base._Ready();

		_achievements = GetNode<Achievements>("/root/Achievements");
	}

	private void StartJump(StringName animName)
	{
		AnimPlayer.AnimationFinished -= StartJump;
		Player.Velocity = new Vector2(_horizontalVelocity, _jumpSpeed);
	}

	internal override void OnEnter(params object[] args)
	{
        _jumpData = new()
        {
            StartPosition = Player.Position
        };

        var jumpType = (StateType) args[0];
		var label = Player.GetNode<Label>("temp_StateName");

		label.Text = jumpType.ToString();
		label.Show();

		if(jumpType == StateType.VerticalJump)
		{
			_horizontalVelocity = 0f;
		}
		else
		{
			_horizontalVelocity = (jumpType == StateType.LowJump ? _lowJumpHorizontalSpeed : _longJumpHorizontalSpeed) * (Player.Velocity.X > 0 ? 1 : -1);
		}

		AudioPlayer.PlayGenericSfx("Jump");

		Player.Velocity = new Vector2(0f, Player.Velocity.Y);
		Player.CanChangeFacingDirection = false;

		AnimPlayer.Play("jump");
		AnimPlayer.AnimationFinished += StartJump;

		_achievements.JumpCount++;
	}

	internal override void PhysicsUpdate(double delta)
	{
		_jumpData.MaxSpeed = new(Mathf.Max(_jumpData.MaxSpeed.X, Player.Velocity.X), Mathf.Min(_jumpData.MaxSpeed.Y, Player.Velocity.Y));
		_jumpData.Duration += (float) delta;

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
			StateMachine.ChangeState("Fall", _jumpData);
		}
		else if(Player.IsOnFloor() && Player.Velocity.Y < 0)
		{
			StateMachine.ChangeState("Idle");
		}
	}

	internal override void OnLeave()
	{
		Player.GetNode<Label>("temp_StateName").Hide();
	}
}
