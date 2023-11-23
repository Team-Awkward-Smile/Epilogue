using Epilogue.actors.hestmor.enums;
using Epilogue.constants;
using Epilogue.nodes;
using Godot;
using System.Threading.Tasks;

namespace Epilogue.actors.hestmor.states;
/// <inheritdoc/>
public partial class Fall : State
{
	private readonly Player _player;
	private bool _playLandingAnimation = true;
	private bool _canGrabLedge;
	private StateType _jumpType;
	private string _animation;

	/// <summary>
	/// 	State that allows Hestmor to fall from high places
	/// </summary>
	/// <param name="stateMachine">The State Machine who owns this State</param>
	public Fall(StateMachine stateMachine) : base(stateMachine) 
	{
		_player = (Player) stateMachine.Owner;
	}

	internal override void OnEnter(params object[] args)
	{
		_jumpType = (StateType) args[0];
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

	internal override void PhysicsUpdate(double delta)
	{
		if(_canGrabLedge && _player.IsOnWall() && _player.SweepForLedge(out var ledgePosition))
		{
			var offset = _player.RayCasts["Head"].GlobalPosition.Y - ledgePosition.Y;

			_playLandingAnimation = false;

			if(offset < -20)
			{
				_player.Position = new Vector2(_player.Position.X, ledgePosition.Y + Constants.MAP_TILE_SIZE);
				StateMachine.ChangeState(typeof(Vault));
			}
			else
			{
				_player.Position -= new Vector2(0f, offset);
				StateMachine.ChangeState(typeof(GrabLedge));
			}

			return;
		}

		_player.Velocity = new Vector2(_player.Velocity.X, _player.Velocity.Y + (StateMachine.Gravity * (float) delta));
		_player.MoveAndSlide();

		if(_player.IsOnFloor())
		{
			StateMachine.ChangeState(typeof(Idle));
		}
	}

	internal override async Task OnLeave()
	{
		if(!_playLandingAnimation)
		{
			return;
		}

		AudioPlayer.PlayGenericSfx("Land");
		AnimPlayer.Play($"Jump/{_animation}_jump_land");

		await StateMachine.ToSignal(AnimPlayer, "animation_finished");
	}
}
