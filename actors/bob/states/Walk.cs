using Epilogue.actors.hestmor.enums;
using Epilogue.global.enums;
using Epilogue.nodes;

using Godot;

namespace Epilogue.actors.hestmor.states;
/// <summary>
///		State that allows Hestmor to walk
/// </summary>
public partial class Walk : PlayerState
{
	[Export] private float _walkSpeed = 100f;

	internal override void OnInput(InputEvent @event)
	{
		if(Input.IsActionJustPressed("jump") && Player.SlowWeight == 0f)
		{
			if(Player.RayCasts["Head"].IsColliding() && !Player.RayCasts["Ledge"].IsColliding())
			{
				StateMachine.ChangeState("GrabLedge");
			}
			else
			{
				StateMachine.ChangeState("Jump", StateType.LowJump);
			}
		}
		else if(Input.IsActionJustPressed("melee"))
		{
			StateMachine.ChangeState("MeleeAttack", StateType.UppercutPunch);
		}
		else if(Input.IsActionJustPressed("crouch"))
		{
			StateMachine.ChangeState("Crouch");
		}
		else if(Input.IsActionJustPressed("slide"))
		{
			StateMachine.ChangeState("Slide", (Player.SlowWeight <= 0.2f ? StateType.KneeSlide : StateType.FrontRoll));
		}
	}

	internal override void OnEnter(params object[] args)
	{
		AnimPlayer.Play("walk", customSpeed: 1f - Player.SlowWeight);

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

			velocity.Y += Player.Gravity * (float) delta;
			velocity.X = movementDirection * _walkSpeed * (float) delta * 60f;

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
		else if(!Player.IsOnFloor() && (Player.Conditions & Conditions.Sinking) == 0)
		{
			StateMachine.ChangeState("Fall", StateType.LongJump);
		}
		else if(Player.RunEnabled && Player.SlowWeight <= 0.2f)
		{
			StateMachine.ChangeState("Run");
		}
		else if(Player.RotationDegrees >= 40f && !goingDownSlope)
		{
			StateMachine.ChangeState("Crawl");
		}
	}
}
