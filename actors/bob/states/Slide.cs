using Epilogue.actors.hestmor.enums;
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

	internal override void OnInput(InputEvent @event)
	{
		if(@event.IsActionPressed(JumpInput))
		{
			StateMachine.ChangeState("Jump", StateType.LongJump);
		}
		else if(@event.IsActionPressed(CancelSlideInput))
		{
			AnimPlayer.Play("slide_end");
			AnimPlayer.AnimationFinished += EndSlide; 
		}
	}

	internal override void OnEnter(params object[] args)
	{
		var label = Player.GetNode<Label>("temp_StateName");
		var speed = (StateType) args[0] switch
		{
			StateType.FrontRoll => _frontRollSpeed,
			StateType.KneeSlide => _kneeSlideSpeed,
			StateType.LongSlide => _longSlideSpeed,
			_ => _longSlideSpeed
		};

		// TODO: 214 - Add a HitBox to the Slide Attack

		label.Text = args[0].ToString();
		label.Show();

		_slideFinished = false;
		_timer = 0f;
		_startingRotation = Player.Rotation;

		var direction = Player.FacingDirection == ActorFacingDirection.Left ? -1 : 1;

		Player.FloorSnapLength = 10f;
		Player.FloorConstantSpeed = false;
		Player.FloorMaxAngle = 0f;
		Player.FloorBlockOnWall = false;
		Player.Velocity = new Vector2(speed * direction, Player.Velocity.Y);
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

		Player.GetNode<Label>("temp_StateName").Hide();
	}

	private void EndSlide(StringName animName)
	{
		AnimPlayer.AnimationFinished -= EndSlide;
		StateMachine.ChangeState("Idle");
	}
}
