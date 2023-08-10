using Epilogue.global.enums;
using Epilogue.global.singletons;
using Epilogue.nodes;
using Godot;

namespace Epilogue.actors.hestmor.states;
/// <summary>
///		State that allows Hestmor to run
/// </summary>
public partial class Run : PlayerState
{
	[Export] private float _runSpeed = 200f;

	private bool _canUseAnalogControls;

	internal override void OnInput(InputEvent @event)
	{
		if(@event.IsActionPressed(JumpInput))
		{
			if(Player.IsOnWall())
			{
				if(Player.RayCasts["Head"].IsColliding())
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
		else if(@event.IsActionPressed(SlideInput))
		{
			StateMachine.ChangeState("Slide");
		}
		else if(@event.IsActionPressed(MeleeAttackInput))
		{
			StateMachine.ChangeState("MeleeAttack");
		}
	}

	internal override void OnEnter()
	{
		AnimPlayer.Play("walk", -1, 2f);

		Player.CanChangeFacingDirection = true;

		_canUseAnalogControls = Settings.ControlScheme == ControlSchemeEnum.Modern;
	}

	internal override void PhysicsUpdate(double delta)
	{
		var movementDirection = Input.GetAxis(MoveLeftDigitalInput, MoveRightDigitalInput);

		if(movementDirection == 0f && _canUseAnalogControls)
		{
			movementDirection = Input.GetAxis(MoveLeftAnalogInput, MoveRightAnalogInput);
		}

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
			StateMachine.ChangeState("Fall");
		}
		else if(!Player.RunEnabled)
		{
			StateMachine.ChangeState("Walk");
		}
		else if(Player.RotationDegrees >= 40f && !goingDownSlope)
		{
			StateMachine.ChangeState("Crawl");
		}
	}
}
