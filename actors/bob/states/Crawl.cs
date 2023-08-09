using Epilogue.global.enums;
using Epilogue.nodes;

using Godot;

namespace Epilogue.actors.hestmor.states;
public partial class Crawl : PlayerState
{
	[Export] private float _crawlSpeed;

	internal override void OnEnter()
	{
		AnimPlayer.Play("crawl");
	}

	internal override void PhysicsUpdate(double delta)
	{
		var movementDirection = Input.GetAxis(MoveLeftDigitalInput, MoveRightDigitalInput);

		if(movementDirection == 0f)
		{
			// KNOWN: 68 - The analog movement works even in Retro Mode
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

		if(!Player.IsOnFloor())
		{
			StateMachine.ChangeState("Fall");
		}
		else if(Player.RotationDegrees < 40f)
		{
			StateMachine.ChangeState("Walk");
		}
	}
}
