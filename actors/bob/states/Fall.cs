using Epilogue.Actors.Hestmor.Enums;
using Epilogue.Const;
using Epilogue.Nodes;
using Godot;
using System.Threading.Tasks;

namespace Epilogue.Actors.Hestmor.States;
/// <inheritdoc/>
public partial class Fall : State
{
	private readonly Player _player;
	private bool _playLandingAnimation = true;
	private bool _canGrabLedge;
	private StateType _jumpType;
	private string _animation;
	private int _frameDelay;
	private bool _slideQueued;
	private double _slideQueueTimer;

	/// <summary>
	/// 	State that allows Hestmor to fall from high places
	/// </summary>
	/// <param name="stateMachine">The State Machine who owns this State</param>
	public Fall(StateMachine stateMachine) : base(stateMachine)
	{
		_player = (Player)stateMachine.Owner;
	}

	internal override void OnInput(InputEvent @event)
	{
		// If the Slide key is pressed mid-air, there will be a 0.15 second time where a Slide will be instantly performed upon landing
		if (@event.IsActionPressed("slide"))
		{
			_slideQueued = true;
		}
	}

	internal override void OnEnter(params object[] args)
	{
		_jumpType = (StateType)args[0];
		_animation = _jumpType switch
		{
			StateType.StandingJump => "vertical",
			_ => "long"
		};

        _canGrabLedge = false;
        _playLandingAnimation = true;

        AnimPlayer.Play($"Jump/{_animation}_jump_down");
        _player.CanChangeFacingDirection = true;

        StateMachine.GetTree().CreateTimer(0.1f).Timeout += () => _canGrabLedge = true;
    }

	internal override void Update(double delta)
	{
		if (_slideQueued && (_slideQueueTimer += delta) >= 0.15)
		{
			_slideQueued = false;
			_slideQueueTimer = 0;
		}
	}

	internal override void PhysicsUpdate(double delta)
	{
		_frameDelay++;

		if (_canGrabLedge && (_frameDelay == 3 || _player.IsOnWall()) && _player.SweepForLedge(out var ledgePosition))
		{
			var offset = _player.RayCasts["Head"].GlobalPosition.Y - ledgePosition.Y;

            _playLandingAnimation = false;

			if (offset < -20)
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

		_frameDelay %= 3;

		_player.Velocity = new Vector2(_player.Velocity.X, _player.Velocity.Y + (StateMachine.Gravity * (float) delta));
		_player.MoveAndSlide();

		if (_player.IsOnFloor())
		{
			if (_slideQueued)
			{
				_playLandingAnimation = false;

				StateMachine.ChangeState(typeof(Slide), StateType.KneeSlide);
			}
			else
			{
				StateMachine.ChangeState(typeof(Idle));
			}
		}
	}

	internal override async Task OnLeave()
	{
		if (!_playLandingAnimation)
		{
			return;
		}

        AudioPlayer.PlayGenericSfx("Land");
        AnimPlayer.Play($"Jump/{_animation}_jump_land");

        await StateMachine.ToSignal(AnimPlayer, "animation_finished");
    }
}
