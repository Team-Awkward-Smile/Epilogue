using Epilogue.actors.hestmor.enums;
using Epilogue.extensions;
using Epilogue.global.enums;
using Epilogue.nodes;
using Godot;

namespace Epilogue.actors.hestmor.states;
/// <summary>
///		State that allows Hestmor to perform slides
/// </summary>
public partial class Slide : PlayerState
{
	[Export] private float _slideTime = 0.5f;
	[Export] private float _longSlideSpeed = 220f;
	[Export] private float _kneeSlideSpeed = 160f;
	[Export] private float _frontRollSpeed = 100f;

	private double _timer = 0f;
	private bool _slideFinished = false;
	private float _startingRotation;
	private string _animation;
	private StateType _rollType;
	private bool _canJump;

	internal override void OnInput(InputEvent @event)
	{
		if(_canJump && Input.IsActionJustPressed("jump"))
		{
			StateMachine.ChangeState("Jump", StateType.LongJump);
		}
		else if(Input.IsActionJustPressed("cancel_slide"))
		{
			AnimPlayer.Play("Slide/slide_end");
			AnimPlayer.AnimationFinished += EndSlide; 
		}
	}

	internal override void OnEnter(params object[] args)
	{
		var horizontalSpeed = 0f;
		var verticalSpeed = 0f;


		_rollType = (StateType) args[0];

		switch(_rollType)
		{
			case StateType.FrontRoll:
				horizontalSpeed = _frontRollSpeed;
				verticalSpeed = -150f;
				_animation = "roll";
				break;

			case StateType.KneeSlide:
				horizontalSpeed = _kneeSlideSpeed;
				_animation = "knee";
				break;

			case StateType.LongSlide or StateType.SlideAttack:
				horizontalSpeed = _longSlideSpeed;
				_animation = "long";
				break;
		}

		// TODO: 214 - Add a HitBox to the Slide Attack

		_slideFinished = false;
		_timer = 0f;
		_startingRotation = Player.Rotation;

		var direction = Player.FacingDirection == ActorFacingDirection.Left ? -1 : 1;

		Player.FloorSnapLength = 10f;
		Player.FloorConstantSpeed = false;
		Player.FloorMaxAngle = 0f;
		Player.FloorBlockOnWall = false;
		Player.Velocity = new Vector2(horizontalSpeed * direction, verticalSpeed);
		Player.CanChangeFacingDirection = false;

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
		Player.Velocity = new Vector2(Player.Velocity.X, Player.Velocity.Y + Gravity * (float) delta);
		Player.MoveAndSlideWithRotation();

		_timer += delta;
		
		if(_rollType != StateType.FrontRoll && _timer > _slideTime && !_slideFinished)
		{
			_canJump = false;
			var raycast = Player.RayCasts["Slide"];

			raycast.Enabled = true;
			raycast.ForceRaycastUpdate();

			// The Slide will only end if Hestmor has enough room to stand up
			if(!raycast.IsColliding())
			{
				_slideFinished = true;

				Player.Velocity = new Vector2(Player.Velocity.X / 2, Player.Velocity.Y);
				AnimPlayer.Play($"Slide/{_animation}_slide_end");
				AnimPlayer.AnimationFinished += EndSlide; 
			}
		}
	}

	internal override void OnLeave()
	{
		AnimPlayer.Reset();

		Player.FloorConstantSpeed = true;
		Player.FloorMaxAngle = Mathf.DegToRad(45f);
		Player.Rotation = _startingRotation;
		Player.FloorBlockOnWall = true;

		_canJump = true;
	}

	private void EndSlide(StringName animName)
	{
		_canJump = false;

		AnimPlayer.AnimationFinished -= EndSlide;
		StateMachine.ChangeState("Idle");
	}
}
