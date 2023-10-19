using Epilogue.actors.hestmor.enums;
using Epilogue.constants;
using Epilogue.nodes;

using Godot;

namespace Epilogue.actors.hestmor.states;
/// <summary>
///		State that allows Hestmor to stay still
/// </summary>
public partial class Idle : PlayerState
{
	[Export] private float _sleepDelay;

	private float _sleepTimer;

	internal override void OnInput(InputEvent @event)
	{
		if(Input.IsActionJustPressed("jump"))
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
			else if(Player.SlowWeight == 0f)
			{
				StateMachine.ChangeState("Jump", StateType.VerticalJump);
			}
		}
		else if(Input.IsActionJustPressed("crouch") && Player.SlowWeight == 0f)
		{
			StateMachine.ChangeState("Crouch");
		}
		else if(Input.IsActionJustPressed("melee"))
		{
			StateMachine.ChangeState("MeleeAttack", StateType.SwipeAttack);
		}
		else if(Input.IsActionJustPressed("slide"))
		{
			StateMachine.ChangeState("Slide", StateType.FrontRoll);
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

	internal override void OnEnter(params object[] args)
	{
		_sleepTimer = 0f;

		Player.CanChangeFacingDirection = true;
		Player.Velocity = new Vector2(0f, 0f);

		AnimPlayer.Play("idle");
	}

	internal override void PhysicsUpdate(double delta)
	{
		_sleepTimer += (float) delta;

		if(_sleepTimer >= _sleepDelay)
		{
			StateMachine.ChangeState("Sleep");
			return;
		}

		if(!Player.IsOnFloor())
		{
			StateMachine.ChangeState("Fall", StateType.LongJump);
			return;
		}

		var movement = Input.GetAxis("move_left", "move_right");

		movement *= 1f - Player.SlowWeight;

		Player.Velocity = new Vector2(movement * 10f, Player.Velocity.Y + Gravity * (float) delta);
		Player.MoveAndSlideWithRotation();

		if(movement != 0f)
		{
			if(Player.IsOnWall() && movement == -Player.GetWallNormal().X)
			{
				return;
			}

			StateMachine.ChangeState(Player.RunEnabled ? "Run" : "Walk");
			return;
		}

		if(Input.IsActionPressed("crouch"))
		{
			StateMachine.ChangeState("Crouch");
			return;
		}
	}
}
