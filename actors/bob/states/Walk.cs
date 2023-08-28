using Epilogue.global.enums;
using Epilogue.global.singletons;
using Epilogue.nodes;
using Godot;

namespace Epilogue.actors.hestmor.states;
/// <summary>
///		State that allows Hestmor to walk
/// </summary>
public partial class Walk : PlayerState
{
	[Export] private float _walkSpeed = 100f;

	private bool _canUseAnalogControls;

	internal override void OnInput(InputEvent @event)
	{
		if(Input.IsActionJustPressed("jump"))
		{
			if(Player.RayCasts["Head"].IsColliding() && !Player.RayCasts["Ledge"].IsColliding())
			{
				StateMachine.ChangeState("GrabLedge");
			}
			else
			{
				StateMachine.ChangeState("Jump");
			}
		}
		else if(Input.IsActionJustPressed("melee"))
		{
			StateMachine.ChangeState("MeleeAttack");
		}
		else if(Input.IsActionJustPressed("crouch"))
		{
			StateMachine.ChangeState("Crouch");
		}
		else if(Input.IsActionJustPressed("toggle_run"))
		{
			StateMachine.ChangeState("Slide");
		}
		else if(Input.IsActionJustPressed("slide"))
		{
			StateMachine.ChangeState("MeleeAttack");
		}

		_canUseAnalogControls = Settings.ControlScheme == ControlSchemeEnum.Modern;
	}

	internal override void OnEnter()
	{
		AnimPlayer.Play("walk");

		Player.CanChangeFacingDirection = true;
	}

	internal override void PhysicsUpdate(double delta)
	{
		var movementDirection = Input.GetAxis("move_left", "move_right");

		if(movementDirection == 0f && _canUseAnalogControls)
		{
			movementDirection = Input.GetAxis(MoveLeftAnalogInput, MoveRightAnalogInput);
		}

		if(movementDirection != 0f)
		{
			movementDirection = movementDirection > 0 ? 1 : -1;

			var velocity = Player.Velocity;

			velocity.Y += Gravity * (float) delta;
			velocity.X = movementDirection * _walkSpeed * (float) delta * 60f;

			if(movementDirection > 0 && Player.FacingDirection == ActorFacingDirection.Left ||
				movementDirection < 0 && Player.FacingDirection == ActorFacingDirection.Right)
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
			StateMachine.ChangeState("Fall");
		}
		else if(Player.RunEnabled)
		{
			StateMachine.ChangeState("Run");
		}
		else if(Player.RotationDegrees >= 40f && !goingDownSlope)
		{
			StateMachine.ChangeState("Crawl");
		}
	}
}
