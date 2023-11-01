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
	private StateType _jumpType;
	private string _animation;
	private int _frameDelay = 0;

	public override void _Ready()
	{
		base._Ready();

		_achievements = GetNode<Achievements>("/root/Achievements");
	}

	private void StartJump(StringName animName)
	{
		var modifier = Player.FacingDirection == ActorFacingDirection.Left ? -1 : 1;

		AnimPlayer.AnimationFinished -= StartJump;
		Player.Velocity = new Vector2(_horizontalVelocity * modifier, _jumpSpeed);
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
		_jumpType = (StateType) args[0];

		switch(_jumpType)
		{
			case StateType.VerticalJump:
				Player.RayCasts["Head"].TargetPosition = new(12f, 0f);
				Player.RayCasts["Ledge"].TargetPosition = new(12f, 0f);
				_horizontalVelocity = 0f;
				_animation = "vertical";
				break;

			case StateType.LowJump:
				_horizontalVelocity = _lowJumpHorizontalSpeed;
				_animation = "long";
				break;

			case StateType.LongJump:
				_horizontalVelocity = _longJumpHorizontalSpeed;
				_animation = "long";
				break;
		}

		AudioPlayer.PlayGenericSfx("Jump");

		Player.Velocity = new Vector2(0f, Player.Velocity.Y);
		Player.CanChangeFacingDirection = false;

		AnimPlayer.Play($"Jump/{_animation}_jump_up");
		AnimPlayer.AnimationFinished += StartJump;

		_achievements.JumpCount++;
	}

	internal override void PhysicsUpdate(double delta)
	{
		_jumpData.MaxSpeed = new(Mathf.Max(_jumpData.MaxSpeed.X, Mathf.Abs(Player.Velocity.X)), Mathf.Max(_jumpData.MaxSpeed.Y, Mathf.Abs(Player.Velocity.Y)));
		_jumpData.Duration += (float) delta;
		_frameDelay++;

		if((_frameDelay == 3 || Player.IsOnWall()) && Player.SweepForLedge(out var ledgePosition))
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

		_frameDelay = _frameDelay >= 3 ? 0 : _frameDelay;

		Player.Velocity = new Vector2(Player.Velocity.X, Player.Velocity.Y + (Gravity * (float) delta));
		Player.MoveAndSlideWithRotation();

		if(Player.Velocity.Y > 0)
		{
			StateMachine.ChangeState("Fall", _jumpType, _jumpData);
		}
		else if(Player.IsOnFloor() && Player.Velocity.Y < 0)
		{
			StateMachine.ChangeState("Idle");
		}
	}

	internal override void OnLeave()
	{
		Player.RayCasts["Head"].TargetPosition = new(8f, 0f);
		Player.RayCasts["Ledge"].TargetPosition = new(8f, 0f);
	}
}
