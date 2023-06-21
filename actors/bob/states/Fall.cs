using Epilogue.extensions;
using Epilogue.nodes;
using Godot;

namespace Epilogue.actors.hestmor.states;
public partial class Fall : StateComponent
{
	public override void OnEnter()
	{
		AnimPlayer.Play("Bob/Jumping_descend");
	}

	public override void PhysicsUpdate(double delta)
	{
		Character.Velocity = new Vector2(Character.Velocity.X, Character.Velocity.Y + (Gravity * (float) delta));
		Character.MoveAndSlide();

		if(Character.IsOnFloor())
		{
			StateMachine.ChangeState("Idle");
		}
		else if(Character.IsOnWall() && !Character.IsHeadRayCastColliding())
		{
			StateMachine.ChangeState("GrabLedge");
		}
	}

	public override void OnLeave()
	{
		AnimPlayer.Play("Bob/Jumping_land");
		AnimPlayer.AnimationFinished += (StringName animName) => EmitSignal(SignalName.StateFinished);
	}
}
