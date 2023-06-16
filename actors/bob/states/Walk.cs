using Epilogue.constants;
using Epilogue.extensions;
using Epilogue.nodes;
using Godot;
using System.Linq;

namespace Epilogue.actors.hestmor.states;
public partial class Walk : StateComponent
{
	[Export] private float _moveSpeed = 100f;

	public override void OnInput(InputEvent @event)
	{
		if(Input.IsActionJustPressed("jump"))
		{
			if(Character.IsOnWall())
			{
				if(Character.IsHeadRayCastColliding())
				{
					// Is near a wall
					StateMachine.ChangeState("Jump");
				}
				else
				{
					// Is near a ledge
					StateMachine.ChangeState("GrabLedge");
				}
			}
			else
			{
				StateMachine.ChangeState("Jump");
			}
		}
		else if(Input.IsActionJustPressed("attack"))
		{
			StateMachine.ChangeState("MeleeAttack");
		}
	}

	public override void OnEnter()
	{
		AnimPlayer.Play("Bob/Walking");
	}

	public override void PhysicsUpdate(double delta)
	{
		var _movementDirection = Input.GetAxis("move_left", "move_right");

		if(_movementDirection == 0f)
		{
			StateMachine.ChangeState("Idle");
			return;
		}

		if(!Character.IsOnFloor())
		{
			StateMachine.ChangeState("Fall");
			return;
		}

		if(Character.IsOnWall() && AnimPlayer.CurrentAnimation == "Bob/Walking")
		{
			AnimPlayer.Play("Bob/Idle");
		}
		else if(!Character.IsOnWall() && AnimPlayer.CurrentAnimation != "Bob/Walking")
		{
			AnimPlayer.Play("Bob/Walking");
		}

		var rotationContainer = (Node2D) Character.GetNode("RotationContainer");
		rotationContainer.Scale = new Vector2(_movementDirection < 0f ? -1 : 1, 1f);

		var velocity = Character.Velocity;

		velocity.Y += Gravity * (float) delta;
		velocity.X = _movementDirection * _moveSpeed;

		Character.Velocity = velocity;
		Character.MoveAndSlide();
	}
}
