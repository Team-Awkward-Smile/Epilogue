using Epilogue.nodes;
using Godot;

namespace Epilogue.actors.hestmor.states;
public partial class Jump : StateComponent
{
	[Export] private float _jumpSpeed = -400f;

	private float _horizontalVelocity;

	private void StartJump(StringName animName)
	{
		AnimPlayer.AnimationFinished -= StartJump;
		Actor.Velocity = new Vector2(_horizontalVelocity, _jumpSpeed);
	}

	public override void OnEnter()
	{
		_horizontalVelocity = Actor.Velocity.X;

		AudioPlayer.PlaySfx("Jump");
		Actor.Velocity = new Vector2(0f, Actor.Velocity.Y);
		AnimPlayer.Play("Bob/Jumping_ascend");
		AnimPlayer.AnimationFinished += StartJump;
	}

	public override void PhysicsUpdate(double delta)
	{
		Actor.Velocity = new Vector2(Actor.Velocity.X, Actor.Velocity.Y + (Gravity * (float) delta));
		Actor.MoveAndSlide();

		if(Actor.Velocity.Y > 0)
		{
			StateMachine.ChangeState("Fall");
			return;
		}
		else if(Actor.IsOnFloor() && Actor.Velocity.Y < 0)
		{
			StateMachine.ChangeState("Idle");
		}
		else if(Actor.IsOnWall() && !Actor.IsHeadRayCastColliding())
		{
			//StateMachine.ChangeState("GrabLedge");
		}
	}
}
