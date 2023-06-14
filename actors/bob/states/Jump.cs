using Epilogue.nodes;
using Godot;

namespace Epilogue.actors.hestmor.states;
public partial class Jump : StateComponent
{
	[Export] private float _jumpSpeed = -400f;

	private float _horizontalVelocity;

	public override void OnEnter()
	{
		_horizontalVelocity = Character.Velocity.X;

		Character.Velocity = new Vector2(0f, Character.Velocity.Y);
		AnimPlayer.Play("Bob/Jumping_ascend");
		AnimPlayer.AnimationFinished += StartJump;
	}

	public override void PhysicsUpdate(double delta)
	{
		Character.Velocity = new Vector2(Character.Velocity.X, Character.Velocity.Y + Gravity * (float) delta);
		Character.MoveAndSlide();

		if(Character.Velocity.Y > 0)
		{
			StateMachine.ChangeState("Fall");
			return;
		}
	}

	private void StartJump(StringName animName)
	{
		Character.Velocity = new Vector2(_horizontalVelocity, _jumpSpeed);
		AnimPlayer.AnimationFinished -= StartJump;
	}
}
