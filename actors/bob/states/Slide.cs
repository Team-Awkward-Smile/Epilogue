using System.Threading.Tasks;
using Epilogue.actors.hestmor.enums;
using Epilogue.Global.Enums;
using Epilogue.nodes;
using Godot;

namespace Epilogue.actors.hestmor.states;
/// <inheritdoc/>
public partial class Slide : State
{
	private readonly float _frontRollDuration;
	private readonly float _longSlideDuration;
	private readonly float _kneeSlideDuration;
	private readonly float _frontRollSpeed;
	private readonly float _longSlideSpeed;
	private readonly float _kneeSlideSpeed;
	private readonly float _frontRollCoyoteDuration;
	private readonly float _longSlideCoyoteDuration;
	private readonly float _kneeSlideCoyoteDuration;
	private readonly Player _player;

	private double _timer;
	private double _coyoteTimer;
	private bool _slideFinished = false;
	private float _startingRotation;
	private string _animation;
	private StateType _rollType;
	private bool _canJump;
	private float _currentSlideDuration;
	private float _currentCoyoteDuration;

	/// <summary>
	/// 	State that allows Hestmor to perform slides
	/// </summary>
	/// <param name="stateMachine">The State Machine who owns this State</param>
	/// <param name="frontRollDuration">The duration (in seconds) of the Front Roll</param>
	/// <param name="longSlideDuration">The duration (in seconds) of the Long Slide</param>
	/// <param name="kneeSlideDuration">The duration (in seconds) of the Knee Slide</param>
	/// <param name="frontRollSpeed">The horizontal speed of the Front Slide</param>
	/// <param name="longSlideSpeed">The horizontal speed of the Long Slide</param>
	/// <param name="kneeSlideSpeed">The horizontal speed of the Knee Slide</param>
	public Slide(
		StateMachine stateMachine,
		float frontRollDuration,
		float longSlideDuration,
		float kneeSlideDuration,
		float frontRollSpeed,
		float longSlideSpeed,
		float kneeSlideSpeed,
		float frontRollcoyoteDuration,
		float longSlideCoyoteDuration,
		float kneeSlideCoyoteDuration) : base(stateMachine)
	{
		_player = (Player) stateMachine.Owner;
		_frontRollDuration = frontRollDuration;
		_longSlideDuration = longSlideDuration;
		_kneeSlideDuration = kneeSlideDuration;
		_frontRollSpeed = frontRollSpeed;
		_longSlideSpeed = longSlideSpeed;
		_kneeSlideSpeed = kneeSlideSpeed;
		_frontRollCoyoteDuration = frontRollcoyoteDuration;
		_longSlideCoyoteDuration = longSlideCoyoteDuration;
		_kneeSlideCoyoteDuration = kneeSlideCoyoteDuration;
	}

	internal override void OnInput(InputEvent @event)
	{
		if(_canJump && Input.IsActionJustPressed("jump"))
		{
			StateMachine.ChangeState(typeof(Jump), StateType.LongJump);
		}
		else if(Input.IsActionJustPressed("cancel_slide"))
		{
			AnimPlayer.Play("Slide/slide_end");
			AnimPlayer.AnimationFinished += EndSlide; 
		}
	}

	internal override void OnEnter(params object[] args)
	{
		var speed = 0f;

		_rollType = (StateType) args[0];

		switch(_rollType)
		{
			case StateType.FrontRoll:
				speed = _frontRollSpeed;
				_animation = "roll";
				_currentSlideDuration = _frontRollDuration;
				_currentCoyoteDuration = _frontRollCoyoteDuration;
				break;

			case StateType.KneeSlide:
				speed = _kneeSlideSpeed;
				_animation = "knee";
				_currentSlideDuration = _kneeSlideDuration;
				_currentCoyoteDuration = _kneeSlideCoyoteDuration;
				break;

			case StateType.LongSlide:
				speed = _longSlideSpeed;
				_animation = "long";
				_currentSlideDuration = _longSlideDuration;
				_currentCoyoteDuration = _longSlideCoyoteDuration;
				break;
		}

		_slideFinished = false;
		_timer = 0f;
		_coyoteTimer = 0f;
		_startingRotation = _player.Rotation;

		var direction = _player.FacingDirection == ActorFacingDirection.Left ? -1 : 1;

		_player.FloorSnapLength = 10f;
		_player.FloorConstantSpeed = false;
		_player.FloorMaxAngle = 0f;
		_player.FloorBlockOnWall = false;
		_player.Velocity = new Vector2(speed * direction, 0f);
		_player.CanChangeFacingDirection = false;

		AnimPlayer.Play($"Slide/{_animation}_slide_start");

		if(_rollType == StateType.FrontRoll)
		{
			_canJump = false;
			AnimPlayer.AnimationFinished += EndSlide;
		}

		AudioPlayer.PlayGenericSfx("Slide");
	}

	internal override void PhysicsUpdate(double delta)
	{
		_timer += delta;
		_coyoteTimer += delta;

		var velocityY = _coyoteTimer >= _currentCoyoteDuration ? _player.Velocity.Y + StateMachine.Gravity * (float)delta : 0f;

        _player.Velocity = new Vector2(_player.Velocity.X, velocityY);
		_player.MoveAndSlide();
		
		if(_rollType != StateType.FrontRoll && _timer > _currentSlideDuration && !_slideFinished)
		{
			_canJump = false;
			_slideFinished = true;

			_player.Velocity = new Vector2(_player.Velocity.X / 2, _player.Velocity.Y);
			AnimPlayer.Play($"Slide/{_animation}_slide_end");
			AnimPlayer.AnimationFinished += EndSlide; 
		}
	}

	internal override Task OnLeave()
	{
		_player.FloorConstantSpeed = true;
		_player.FloorMaxAngle = Mathf.DegToRad(45f);
		_player.Rotation = _startingRotation;
		_player.FloorBlockOnWall = true;
		_canJump = true;

		return Task.CompletedTask;
	}

	private void EndSlide(StringName animName)
	{
		_canJump = false;

		AnimPlayer.AnimationFinished -= EndSlide;
		StateMachine.ChangeState(typeof(Idle));
	}
}
