using Epilogue.global.enums;
using Epilogue.global.singletons;
using Epilogue.nodes;

using Godot;

namespace Epilogue.actors.hestmor.states;
public partial class Crawl : PlayerState
{
	[Export] private float _crawlSpeed;

	private bool _canUseAnalogControls;

	internal override void OnEnter()
	{
		AnimPlayer.Play("crawl");

		_canUseAnalogControls = Settings.ControlScheme == ControlSchemeEnum.Modern;
	}

	internal override void PhysicsUpdate(double delta)
	{
		var movementDirection = Input.GetAxis(MoveLeftDigitalInput, MoveRightDigitalInput);

		if(movementDirection == 0f && _canUseAnalogControls)
		{
			movementDirection = Input.GetAxis(MoveLeftAnalogInput, MoveRightAnalogInput);
		}

		if(movementDirection == 0f)
		{
			AnimPlayer.Pause();
		}
		else if(!AnimPlayer.IsPlaying())
		{
			AnimPlayer.Play();
		}

		var velocity = Player.Velocity;

		velocity.Y += Gravity * (float) delta;
		velocity.X = movementDirection * _crawlSpeed * (float) delta * 60f;

		Player.Velocity = velocity;

		Player.MoveAndSlideWithRotation();

		var slopeNormal = Player.GetFloorNormal();
		var goingDownSlope = (movementDirection < 0 && slopeNormal.X < 0) || (movementDirection > 0 && slopeNormal.X > 0);

		if(!Player.IsOnFloor())
		{
			StateMachine.ChangeState("Fall");
		}
		else if(goingDownSlope || Player.RotationDegrees < 40f)
		{
			StateMachine.ChangeState("Idle");
		}
	}
}