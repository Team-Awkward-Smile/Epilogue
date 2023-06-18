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
		else if(Input.IsActionJustPressed("attack"))
		{
			StateMachine.ChangeState("MeleeAttack");
		}
	}

	public override void OnEnter()
	{
		AnimPlayer.Play("Bob/Idle");
	}

	public override void PhysicsUpdate(double delta)
	{
		if(!Character.IsOnFloor())
		{
			StateMachine.ChangeState("Fall");
			return;
		}

		var movement = Input.GetAxis("move_left", "move_right"); 
		if(movement != 0f)
		{
			if(Character.IsOnWall() && movement == -Character.GetWallNormal().X)
			{
				return;
			}

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
