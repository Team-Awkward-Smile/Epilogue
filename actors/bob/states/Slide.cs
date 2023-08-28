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
	[Export] private float _slideSpeed = 220f;

	private double _timer = 0f;
	private bool _slideFinished = false;
	private float _startingRotation;

	internal override void OnInput(InputEvent @event)
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

	internal override void OnEnter()
	{
		_slideFinished = false;
		_timer = 0f;
		_startingRotation = Player.Rotation;

		var direction = Player.FacingDirection == ActorFacingDirection.Left ? -1 : 1;

		Player.FloorSnapLength = 10f;
		Player.FloorConstantSpeed = false;
		Player.FloorMaxAngle = 0f;
		Player.FloorBlockOnWall = false;
		Player.Velocity = new Vector2(_slideSpeed * direction, Player.Velocity.Y);
		Player.CanChangeFacingDirection = false;

		AnimPlayer.Play("slide_start");

		AudioPlayer.PlayGenericSfx("Slide");
	}

	internal override void PhysicsUpdate(double delta)
	{
		Player.Velocity = new Vector2(Player.Velocity.X, Player.Velocity.Y + Gravity * (float) delta);
		Player.MoveAndSlideWithRotation();

		_timer += delta;
		
		if(_timer > _slideTime && !_slideFinished)
		{
			_slideFinished = true;

			Player.Velocity = new Vector2(Player.Velocity.X / 2, Player.Velocity.Y);
			AnimPlayer.Play("slide_end");
			AnimPlayer.AnimationFinished += EndSlide; 
		}
	}

	internal override void OnLeave()
	{
		Player.FloorConstantSpeed = true;
		Player.FloorMaxAngle = Mathf.DegToRad(45f);
		Player.Rotation = _startingRotation;
		Player.FloorBlockOnWall = true;
	}

	private void EndSlide(StringName animName)
	{
		AnimPlayer.AnimationFinished -= EndSlide;
		StateMachine.ChangeState("Idle");
	}
}
