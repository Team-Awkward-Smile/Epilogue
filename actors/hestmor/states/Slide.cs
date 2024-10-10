using System.Threading.Tasks;
using Epilogue.Actors.Hestmor.Enums;
using Epilogue.Global.Enums;
using Epilogue.Nodes;
using Godot;

namespace Epilogue.Actors.Hestmor.States;
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
	private ShapeCast2D _slideShapeCast2D;
	private RayCast2D _slideEndRayCast2D;

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
	/// <param name="frontRollcoyoteDuration">Duration of the Coyote Time of the Front Slide</param>
	/// <param name="longSlideCoyoteDuration">Duration of the Coyote Time of the Long Slide</param>
	/// <param name="kneeSlideCoyoteDuration">Duration of the Coyote Time of the Knee Slide</param>
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
		_player = (Player)stateMachine.Owner;
		_frontRollDuration = frontRollDuration;
		_longSlideDuration = longSlideDuration;
		_kneeSlideDuration = kneeSlideDuration;
		_frontRollSpeed = frontRollSpeed;
		_longSlideSpeed = longSlideSpeed;
		_kneeSlideSpeed = kneeSlideSpeed;
		_frontRollCoyoteDuration = frontRollcoyoteDuration;
		_longSlideCoyoteDuration = longSlideCoyoteDuration;
		_kneeSlideCoyoteDuration = kneeSlideCoyoteDuration;

		SpriteSheetId = (int)Enums.SpriteSheetId.Bob;
	}

	internal override void OnStateMachineActivation()
	{
		_slideShapeCast2D = _player.ShapeCasts["Slide"];
		_slideEndRayCast2D = _player.RayCasts["SlideEnd"];

		AnimPlayer.AnimationFinished += (StringName animationName) =>
		{
			if (!Active || !animationName.ToString().EndsWith("slide_end"))
			{
				return;
			}

			_canJump = false;

			StateMachine.ChangeState(typeof(Idle));
		};

		AnimPlayer.AnimationFinished += (StringName animationName) =>
		{
			if (!Active || animationName != "Slide/front_roll")
			{
				return;
			}

			_canJump = false;

			_slideShapeCast2D.ForceShapecastUpdate();

			StateMachine.ChangeState(_slideShapeCast2D.IsColliding() || Input.IsActionPressed("crouch_squat") ? typeof(Crawl) : typeof(Idle));
		};

	}

	internal override void OnInput(InputEvent @event)
	{
		if (_canJump && @event.IsActionPressed("jump"))
		{
			StateMachine.ChangeState(typeof(Jump), _player.IsOnFloor() ? StateType.LongJump : StateType.CoyoteJump);
		}
		else if (@event.IsActionPressed("cancel_slide"))
		{
			_slideShapeCast2D.ForceShapecastUpdate();

			if (!_slideShapeCast2D.IsColliding())
			{
				AnimPlayer.Play("Slide/slide_end");
			}
			else
			{
				StateMachine.ChangeState(typeof(Crawl));
			}
		}
	}

	// args[0] - StateType - Type of Slide to be performed
	internal override void OnEnter(params object[] args)
	{
		_slideEndRayCast2D.Enabled = _slideShapeCast2D.Enabled = true;

		var speed = 0f;

		_rollType = (StateType)args[0];

		switch (_rollType)
		{
			case StateType.FrontRoll:
				speed = _frontRollSpeed;
				_animation = "front_roll";
				_currentSlideDuration = _frontRollDuration;
				_currentCoyoteDuration = _frontRollCoyoteDuration;
				_footstepManager.PlayRandomCollisionSfx("Roll");
				break;

			case StateType.KneeSlide:
				speed = _kneeSlideSpeed;
				_animation = "knee_slide";
				_currentSlideDuration = _kneeSlideDuration;
				_currentCoyoteDuration = _kneeSlideCoyoteDuration;
				_footstepManager.PlayRandomCollisionSfx("Slide");
				break;

			case StateType.LongSlide:
				speed = _longSlideSpeed;
				_animation = "long_slide";
				_currentSlideDuration = _longSlideDuration;
				_currentCoyoteDuration = _longSlideCoyoteDuration;
				_footstepManager.PlayRandomCollisionSfx("Slide");
				break;
		}

		_slideFinished = false;
		_timer = 0f;
		_coyoteTimer = 0f;
		_startingRotation = _player.Rotation;
		_canJump = _rollType != StateType.FrontRoll;

		var direction = _player.FacingDirection == ActorFacingDirection.Left ? -1 : 1;

		_player.FloorSnapLength = 10f;
		_player.FloorConstantSpeed = false;
		_player.FloorMaxAngle = 0f;
		_player.FloorBlockOnWall = false;
		_player.Velocity = new Vector2(speed * direction, 0f);
		_player.CanChangeFacingDirection = false;

		AnimPlayer.Play($"Slide/{(_rollType != StateType.FrontRoll ? _animation + "_start" : _animation)}");
		AudioPlayer.PlayGenericSfx("Slide");
		
	}

	internal override void PhysicsUpdate(double delta)
	{
		_timer += delta;

		var velocityY = _coyoteTimer >= _currentCoyoteDuration ? _player.Velocity.Y + StateMachine.Gravity * (float)delta : 0f;

		_player.Velocity = new Vector2(_player.Velocity.X, velocityY);

		_player.MoveAndSlide();

		if (!_player.IsOnFloor() && (_coyoteTimer += delta) > _currentCoyoteDuration)
		{
			_canJump = false;
		}

		if (_rollType != StateType.FrontRoll && _timer > _currentSlideDuration && !_slideFinished)
		{
			_slideEndRayCast2D.ForceRaycastUpdate();
			_slideShapeCast2D.ForceShapecastUpdate();

			var slideCanEnd = !_slideShapeCast2D.IsColliding();

			if (!slideCanEnd && !_slideEndRayCast2D.IsColliding()) 
			{
				_timer -= 0.3f;

				return;
			}

			if (slideCanEnd)
			{
				_canJump = false;
				_slideFinished = true;

				_player.Velocity = new Vector2(_player.Velocity.X / 2, _player.Velocity.Y);

				AnimPlayer.Play($"Slide/{_animation}_end");
			}
			else
			{
				StateMachine.ChangeState(typeof(Crawl));
			}
		}
	}

	internal override Task OnLeave()
	{
		_player.FloorConstantSpeed = true;
		_player.FloorMaxAngle = Mathf.DegToRad(45f);
		_player.Rotation = _startingRotation;
		_player.FloorBlockOnWall = true;

		_canJump = true;
		_player.CanChangeFacingDirection = true;
		_slideEndRayCast2D.Enabled = _slideShapeCast2D.Enabled = false;

		return Task.CompletedTask;
	}
}

