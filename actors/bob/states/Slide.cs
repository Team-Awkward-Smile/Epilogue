using Epilogue.nodes;
using Godot;

namespace Epilogue.actors.hestmor.states;
public partial class Slide : StateComponent
{
	[Export] private float _slideTime = 0.5f;
	[Export] private float _slideSpeedBonus = 0.1f;

	private double _timer = 0f;
	private bool _slideFinished = false;
	private float _slopeSnap;

	public override void OnInput(InputEvent @event)
	{
		if(Input.IsActionJustPressed("jump"))
		{
			StateMachine.ChangeState("Jump");
		}
		else if(Input.IsActionJustPressed("up"))
		{
			AnimPlayer.Play("Bob/Slide_end");
			AnimPlayer.AnimationFinished += (StringName animName) =>
			{
				StateMachine.ChangeState("Idle");
			};
		}
	}

	public override void OnEnter()
	{
		EmitSignal(SignalName.StateStarted);

		_slopeSnap = Character.FloorSnapLength;

		Character.FloorSnapLength = 10f;
		Character.FloorConstantSpeed = false;
		Character.Velocity = new Vector2(Character.Velocity.X * (1f + _slideSpeedBonus), Character.Velocity.Y);
		AnimPlayer.Play("Bob/Slide_start");
	}

	public override void PhysicsUpdate(double delta)
	{
		Character.Velocity = new Vector2(Character.Velocity.X, Character.Velocity.Y + Gravity * (float) delta);
		Character.MoveAndSlide();

		_timer += delta;
		
		if(_timer > _slideTime && !_slideFinished)
		{
			_slideFinished = true;

			Character.Velocity = new Vector2(Character.Velocity.X / 2, Character.Velocity.Y);
			AnimPlayer.Play("Bob/Slide_end");
			AnimPlayer.AnimationFinished += (StringName animName) =>
			{
				StateMachine.ChangeState("Idle");
			};
		}
	}

	public override void OnLeave()
	{
		Character.FloorSnapLength = _slopeSnap;
		Character.FloorConstantSpeed = true;

		EmitSignal(SignalName.StateFinished);
	}
}
