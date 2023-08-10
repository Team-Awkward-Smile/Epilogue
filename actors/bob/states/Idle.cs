using Epilogue.constants;
using Epilogue.global.enums;
using Epilogue.global.singletons;
using Epilogue.nodes;
using Godot;

namespace Epilogue.actors.hestmor.states;
/// <summary>
///		State that allows Hestmor to stay still
/// </summary>
public partial class Idle : PlayerState
{
	private bool _canUseAnalogControls;

	internal override void OnInput(InputEvent @event)
	{
		if(@event.IsActionPressed(JumpInput))
		{
			if(!Player.RayCasts["Head"].IsColliding() && Player.RayCasts["Feet"].IsColliding())
			{
				var raycast = Player.RayCasts["Ledge"];
				var originalPosition = raycast.Position;

				raycast.Position = new Vector2(0f, -Constants.MAP_TILE_SIZE - 1);

				raycast.ForceRaycastUpdate();

				if(!raycast.IsColliding())
				{
					StateMachine.ChangeState("Vault");
				}

				raycast.Position = originalPosition;
			}
			else
			{
				StateMachine.ChangeState("Jump");
			}
		}
		else if(@event.IsActionPressed(CrouchInput))
		{
			StateMachine.ChangeState("Crouch");
		}
		else if(@event.IsActionPressed(SlideInput))
		{
			StateMachine.ChangeState("Slide");
		}
		else if(@event.IsActionPressed(LookUpInput))
		{
			StateMachine.ChangeState("LookUp");
		}
		else if(@event.IsActionPressed(MeleeAttackInput))
		{
			StateMachine.ChangeState("MeleeAttack");
		}
		else if(@event.IsActionPressed(GrowlInput))
		{
			StateMachine.ChangeState("Growl");
		}
	}

	internal override void OnEnter()
	{
		Player.CanChangeFacingDirection = true;
		Player.Velocity = new Vector2(0f, 0f);

		AnimPlayer.Play("idle");

		_canUseAnalogControls = Settings.ControlScheme == ControlSchemeEnum.Modern;
	}

	internal override void PhysicsUpdate(double delta)
	{
		if(!Player.IsOnFloor())
		{
			StateMachine.ChangeState("Fall");
			return;
		}

		var movement = Input.GetAxis(MoveLeftDigitalInput, MoveRightDigitalInput);

		if(movement == 0f && _canUseAnalogControls)
		{
			movement = Input.GetAxis(MoveLeftAnalogInput, MoveRightAnalogInput);
		}

		if(movement != 0f)
		{
			if(Player.IsOnWall() && movement == -Player.GetWallNormal().X)
			{
				return;
			}

			StateMachine.ChangeState(Player.RunEnabled ? "Run" : "Walk");
			return;
		}

		if(Input.IsActionPressed(CrouchInput))
		{
			StateMachine.ChangeState("Crouch");
			return;
		}
	}
}
