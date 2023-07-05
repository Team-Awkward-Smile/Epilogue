using Epilogue.global.enums;
using Epilogue.global.singletons;
using Epilogue.nodes;
using Godot;
using System;

namespace Epilogue.actors.hestmor.states;
public partial class Idle : StateComponent
{
	public override void OnInput(InputEvent @event)
	{
		if(Input.IsActionJustPressed(_jumpInput))
		{
			StateMachine.ChangeState("Jump");
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
	}

	public override void OnEnter()
	{
		Actor.Velocity = new Vector2(0f, 0f);
		AnimPlayer.Play("Bob/Idle");
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
