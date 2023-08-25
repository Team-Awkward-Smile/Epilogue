using Epilogue.nodes;
using Godot;

namespace Epilogue.actors.hestmor.states;
public partial class Idle : PlayerState
{
	public override void OnInput(InputEvent @event)
	{
		if(Input.IsActionJustPressed("jump"))
		{
			if(Actor.IsOnWall() && !Actor.RayCasts["Head"].IsColliding() && !Actor.RayCasts["Ledge"].IsColliding())
			{
				StateMachine.ChangeState("Vault");
			}
			else
			{
				StateMachine.ChangeState("Jump");
			}
		}
		else if(Input.IsActionJustPressed("crouch"))
		{
			StateMachine.ChangeState("Crouch");
		}
		else if(Input.IsActionJustPressed("melee"))
		{
			StateMachine.ChangeState("MeleeAttack");
		}
		else if(Input.IsActionJustPressed("slide"))
		{
			StateMachine.ChangeState("Slide");
		}
		else if(Input.IsActionJustPressed("look_up"))
		{
			StateMachine.ChangeState("LookUp");
		}
		else if(Input.IsActionJustPressed("growl"))
		{
			StateMachine.ChangeState("Growl");
		}
	}

	public override void OnEnter()
	{
		Actor.CanChangeFacingDirection = true;

		AnimPlayer.Play("idle");
	}

	public override void PhysicsUpdate(double delta)
	{
		if(!Actor.IsOnFloor())
		{
			StateMachine.ChangeState("Fall");
			return;
		}

		var movement = Input.GetAxis("move_left", "move_right");

		if(movement != 0f)
		{
			if(Actor.IsOnWall() && movement == -Actor.GetWallNormal().X)
			{
				return;
			}

			StateMachine.ChangeState(Input.IsActionPressed("toggle_run") ? "Run" : "Walk");
			return;
		}

		if(Input.IsActionPressed("crouch"))
		{
			StateMachine.ChangeState("Crouch");
			return;
		}
	}
}
