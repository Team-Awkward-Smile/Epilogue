using Epilogue.global.enums;
using Epilogue.nodes;
using Godot;

namespace Epilogue.actors.hestmor.states;
public partial class Slide : PlayerState
{
	[Export] private float _slideTime = 0.5f;
	[Export] private float _slideSpeed = 220f;

	private double _timer = 0f;
	private bool _slideFinished = false;
	private float _startingRotation;

	public override void OnInput(InputEvent @event)
	{
		if(Input.IsActionJustPressed("jump"))
		{
			StateMachine.ChangeState("Jump");
		}
		else if(Input.IsActionJustPressed("cancel_slide"))
		{
			AnimPlayer.Play("slide_end");
			AnimPlayer.AnimationFinished += EndSlide; 
		}
	}

	public override void OnEnter()
	{
		_slideFinished = false;
		_timer = 0f;
		_startingRotation = Actor.Rotation;

		var direction = Actor.FacingDirection == ActorFacingDirectionEnum.Left ? -1 : 1;

		Actor.FloorSnapLength = 10f;
		Actor.FloorConstantSpeed = false;
		Actor.FloorMaxAngle = 0f;
		Actor.FloorBlockOnWall = false;
		Actor.Velocity = new Vector2(_slideSpeed * direction, Actor.Velocity.Y);
		Actor.CanChangeFacingDirection = false;

		AnimPlayer.Play("slide_start");

		AudioPlayer.PlayGenericSfx("Slide");
	}

	public override void PhysicsUpdate(double delta)
	{
		Actor.Velocity = new Vector2(Actor.Velocity.X, Actor.Velocity.Y + Gravity * (float) delta);
		Actor.MoveAndSlideWithRotation();

		_timer += delta;
		
		if(_timer > _slideTime && !_slideFinished)
		{
			_slideFinished = true;

			Actor.Velocity = new Vector2(Actor.Velocity.X / 2, Actor.Velocity.Y);
			AnimPlayer.Play("slide_end");
			AnimPlayer.AnimationFinished += EndSlide; 
		}
	}

	public override void OnLeave()
	{
		Actor.FloorConstantSpeed = true;
		Actor.FloorMaxAngle = Mathf.DegToRad(45f);
		Actor.Rotation = _startingRotation;
		Actor.FloorBlockOnWall = true;
	}

	private void EndSlide(StringName animName)
	{
		AnimPlayer.AnimationFinished -= EndSlide;
		StateMachine.ChangeState("Idle");
	}
}
