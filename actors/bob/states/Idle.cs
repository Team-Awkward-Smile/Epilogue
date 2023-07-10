using Epilogue.nodes;
using Godot;

namespace Epilogue.actors.hestmor.states;
public partial class Idle : PlayerState
{
	public override void OnInput(InputEvent @event)
	{
		if(Input.IsActionJustPressed(_jumpInput))
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
		else if(Input.IsActionJustPressed(_crouchInput))
		{
			StateMachine.ChangeState("Crouch");
		}
		else if(Input.IsActionJustPressed(_attackInput))
		{
			StateMachine.ChangeState("MeleeAttack");
		}
		else if(Input.IsActionJustPressed(_slideInput))
		{
			StateMachine.ChangeState("Slide");
		}
		else if(Input.IsActionJustPressed(_lookUpInput))
		{
			StateMachine.ChangeState("LookUp");
		}
		else if(Input.IsActionJustPressed(_growlInput))
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

		var movement = Input.GetAxis(_moveLeftInput, _moveRightInput);

		if(movement != 0f)
		{
			if(Actor.IsOnWall() && movement == -Actor.GetWallNormal().X)
			{
				return;
			}

			StateMachine.ChangeState(Input.IsActionPressed(_toggleRunInput) ? "Run" : "Walk");
			return;
		}

		if(Input.IsActionPressed(_crouchInput))
		{
			StateMachine.ChangeState("Crouch");
			return;
		}
	}
}
