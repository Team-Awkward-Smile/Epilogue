using Epilogue.global.enums;
using Epilogue.nodes;
using Godot;

namespace Epilogue.actors.hestmor.states;
public partial class Slide : StateComponent
{
	[Export] private float _slideTime = 0.5f;
	[Export] private float _slideSpeed = 220f;

	private double _timer = 0f;
	private bool _slideFinished = false;

	public override void OnInput(InputEvent @event)
	{
		if(Input.IsActionJustPressed(_jumpInput))
		{
			StateMachine.ChangeState("Jump");
		}
		else if(Input.IsActionJustPressed(_cancelSlideInput))
		{
			AnimPlayer.Play("Bob/Slide_end");
			AnimPlayer.AnimationFinished += EndSlide; 
		}
	}

	public override void OnEnter()
	{
		EmitSignal(SignalName.StateStarted);

		_slideFinished = false;
		_timer = 0f;

		var direction = Actor.FacingDirection == ActorFacingDirectionEnum.Left ? -1 : 1;

		Actor.FloorSnapLength = 10f;
		Actor.FloorConstantSpeed = false;
		Actor.FloorMaxAngle = 0f;
		Actor.Velocity = new Vector2(_slideSpeed * direction, Actor.Velocity.Y);
		AnimPlayer.Play("Bob/Slide_start");
		AudioPlayer.PlaySfx("Slide");
	}

	public override void PhysicsUpdate(double delta)
	{
		Actor.Velocity = new Vector2(Actor.Velocity.X, Actor.Velocity.Y + Gravity * (float) delta);
		Actor.MoveAndSlide();

		_timer += delta;
		
		if(_timer > _slideTime && !_slideFinished)
		{
			_slideFinished = true;

			Actor.Velocity = new Vector2(Actor.Velocity.X / 2, Actor.Velocity.Y);
			AnimPlayer.Play("Bob/Slide_end");
			AnimPlayer.AnimationFinished += EndSlide; 
		}
	}

	public override void OnLeave()
	{
		Actor.FloorConstantSpeed = true;
		Actor.FloorMaxAngle = 45f;

		EmitSignal(SignalName.StateFinished);
	}

	private void EndSlide(StringName animName)
	{
		AnimPlayer.AnimationFinished -= EndSlide;
		StateMachine.ChangeState("Idle");
	}
}
