using Epilogue.actors.hestmor.enums;
using Epilogue.global.enums;
using Epilogue.nodes;

using Godot;

namespace Epilogue.actors.hestmor.states;
/// <summary>
///		State that allows Hestmor to run
/// </summary>
public partial class Run : PlayerState
{
	[Export] private float _runSpeed = 200f;

	internal override void OnInput(InputEvent @event)
	{
		if(Input.IsActionJustPressed("jump") && Player.SlowWeight == 0f)
		{
			if(Player.IsOnWall())
			{
				if(Player.RayCasts["Head"].IsColliding())
				{
					// Is near a wall
					StateMachine.ChangeState("Jump", StateType.LongJump);
				}
				else
				{
					// Is near a ledge
					StateMachine.ChangeState("GrabLedge");
				}
			}
			else
			{
				StateMachine.ChangeState("Jump", StateType.LongJump);
			}
		}
		else if(Input.IsActionJustPressed("melee"))
		{
			StateMachine.ChangeState("Slide", StateType.SlideAttack);
		}
		else if(Input.IsActionJustPressed("slide"))
		{
			StateMachine.ChangeState("Slide", StateType.LongSlide);
		}
	}

	internal override void OnEnter(params object[] args)
	{
		AnimPlayer.Play("walk", -1, 2f);

		Player.CanChangeFacingDirection = true;
	}

	internal override void PhysicsUpdate(double delta)
	{
		var movementDirection = Input.GetAxis("move_left", "move_right");

		movementDirection = Mathf.Round(movementDirection);
		movementDirection *= 1f - Player.SlowWeight;

		if(movementDirection != 0f)
		{
			var velocity = Player.Velocity;

			velocity.Y += Gravity * (float) delta;
			velocity.X = movementDirection * _runSpeed;

			if((movementDirection > 0 && Player.FacingDirection == ActorFacingDirection.Left) ||
				(movementDirection < 0 && Player.FacingDirection == ActorFacingDirection.Right))
			{
				velocity.X /= 2;
			}

			Player.Velocity = velocity;
		}

		Player.MoveAndSlideWithRotation();

		var floorNormal = Player.GetFloorNormal();
		var goingDownSlope = (movementDirection < 0 && floorNormal.X < 0) || (movementDirection > 0 && floorNormal.X > 0);

		if(movementDirection == 0f || Player.IsOnWall())
		{
			StateMachine.ChangeState("Idle");
		}
		else if(!Player.IsOnFloor())
		{
			StateMachine.ChangeState("Fall", StateType.LongJump);
		}
		else if(!Player.RunEnabled || Player.SlowWeight > 0.2f)
		{
			StateMachine.ChangeState("Walk");
		}
		else if(Player.RotationDegrees >= 40f && !goingDownSlope)
		{
			StateMachine.ChangeState("Crawl");
		}
	}
}
