using System.Threading.Tasks;
using Epilogue.Actors.Hestmor.Enums;
using Epilogue.Const;
using Epilogue.Global.Enums;
using Epilogue.Global.Singletons;
using Epilogue.Nodes;
using Godot;

namespace Epilogue.Actors.Hestmor.States;
/// <inheritdoc/>
public partial class Jump : State
{
	private readonly float _standingJumpVerticalSpeed;
	private readonly float _lowJumpVerticalSpeed;
	private readonly float _longJumpVerticalSpeed;
	private readonly float _lowJumpHorizontalSpeed;
	private readonly float _longJumpHorizontalSpeed;
	private readonly Player _player;
	private readonly Achievements _achievements;

	private float _horizontalVelocity;
	private StateType _jumpType;
	private string _animation;
	private int _frameDelay = 0;
	private float _currentJumpVerticalSpeed;

	/// <summary>
	/// 	State that allows Hestmor to start a jump
	/// </summary>
	/// <param name="stateMachine">The State Machine who owns this State</param>
	/// <param name="standingJumpVerticalSpeed">The vertical speed of the Standing Jump</param>
	/// <param name="lowJumpVerticalSpeed">The vertical speed of the Low Jump</param>
	/// <param name="lowJumpHorizontalSpeed">The horizontal speed of the Low Jump</param>
	/// <param name="longJumpVerticalSpeed">The vertical speed of the Long Jump</param>
	/// <param name="longJumpHorizontalSpeed">The horizontal speed of the Long Jump</param>
	public Jump(
		StateMachine stateMachine,
		float standingJumpVerticalSpeed,
		float lowJumpVerticalSpeed,
		float lowJumpHorizontalSpeed,
		float longJumpVerticalSpeed,
		float longJumpHorizontalSpeed) : base(stateMachine)
	{
		_player = (Player)stateMachine.Owner;
		_achievements = StateMachine.GetNode<Achievements>("/root/Achievements");
		_standingJumpVerticalSpeed = standingJumpVerticalSpeed;
		_lowJumpVerticalSpeed = lowJumpVerticalSpeed;
		_lowJumpHorizontalSpeed = lowJumpHorizontalSpeed;
		_longJumpHorizontalSpeed = longJumpHorizontalSpeed;
		_longJumpVerticalSpeed = longJumpVerticalSpeed;
	}

	private void StartJump(StringName animName)
	{
		var modifier = _player.FacingDirection == ActorFacingDirection.Left ? -1 : 1;

		AnimPlayer.AnimationFinished -= StartJump;
		_player.Velocity = new Vector2(_horizontalVelocity * modifier, _currentJumpVerticalSpeed);
	}

	internal override void OnEnter(params object[] args)
	{
		_jumpType = (StateType)args[0];

		switch (_jumpType)
		{
			case StateType.StandingJump:
				_player.RayCasts["Head"].TargetPosition = new(12f, 0f);
				_player.RayCasts["Ledge"].TargetPosition = new(12f, 0f);
				_horizontalVelocity = 0f;
				_animation = "vertical";
				_currentJumpVerticalSpeed = _standingJumpVerticalSpeed;
				break;

			case StateType.LowJump:
				_horizontalVelocity = _lowJumpHorizontalSpeed;
				_currentJumpVerticalSpeed = _lowJumpVerticalSpeed;
				_animation = "long";
				break;

			case StateType.LongJump:
				_horizontalVelocity = _longJumpHorizontalSpeed;
				_currentJumpVerticalSpeed = _longJumpVerticalSpeed;
				_animation = "long";
				break;
		}

		AudioPlayer.PlayGenericSfx("Jump");

		_player.Velocity = new Vector2(0f, _player.Velocity.Y);
		_player.CanChangeFacingDirection = false;

		AnimPlayer.Play($"Jump/{_animation}_jump_up", customSpeed: 2);
		AnimPlayer.AnimationFinished += StartJump;

		_achievements.JumpCount++;
	}

	internal override void PhysicsUpdate(double delta)
	{
		_frameDelay++;

		if ((_frameDelay == 3 || _player.IsOnWall()) && _player.SweepForLedge(out Vector2 ledgePosition))
		{
			var offset = _player.RayCasts["Head"].GlobalPosition.Y - ledgePosition.Y;

			if (offset < -30)
			{
				_player.Position = new Vector2(_player.Position.X, ledgePosition.Y + Const.Constants.MAP_TILE_SIZE);
				StateMachine.ChangeState(typeof(Vault));
			}
			else
			{
				_player.Position -= new Vector2(0f, offset);
				StateMachine.ChangeState(typeof(GrabLedge));
			}

			return;
		}

		_frameDelay = _frameDelay >= 3 ? 0 : _frameDelay;

		_player.Velocity = new Vector2(_player.Velocity.X, _player.Velocity.Y + (StateMachine.Gravity * (float) delta));
		_player.MoveAndSlide();

		if (_player.Velocity.Y > 0)
		{
			StateMachine.ChangeState(typeof(Fall), _jumpType);
		}
		else if (_player.IsOnFloor() && _player.Velocity.Y < 0)
		{
			StateMachine.ChangeState(typeof(Idle));
		}
	}

	internal override Task OnLeave()
	{
		_player.RayCasts["Head"].TargetPosition = new(8f, 0f);
		_player.RayCasts["Ledge"].TargetPosition = new(8f, 0f);

		return Task.CompletedTask;
	}
}
