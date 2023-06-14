using Epilogue.nodes;
using Godot;

namespace Epilogue.actors.hestmor.states;
public partial class Idle : StateComponent
{
	public override void OnInput(InputEvent @event)
	{
		if(Input.IsActionJustPressed("jump"))
		{
			StateMachine.ChangeState("Jump");
		}
		else if(Input.IsActionJustPressed("crouch"))
		{
			StateMachine.ChangeState("Crouch");
		}
	}

	public override void OnEnter()
	{
		AnimPlayer.Play("Bob/Idle");
	}

	public override void PhysicsUpdate(double delta)
	{
		Character.Velocity = new Vector2(0f, Character.Velocity.Y + (Gravity * (float) delta));
		Character.MoveAndSlide();

		if(!Character.IsOnFloor())
		{
			StateMachine.ChangeState("Fall");
			return;
		}

		if(Input.GetAxis("move_left", "move_right") != 0f)
		{
			StateMachine.ChangeState("Walk");
			return;
		}

		if(Input.IsActionPressed("crouch"))
		{
			StateMachine.ChangeState("Crouch");
			return;
		}
	}
}
