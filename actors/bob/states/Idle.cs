using Epilogue.constants;
using Epilogue.nodes;

using Godot;

namespace Epilogue.actors.hestmor.states;
/// <summary>
///		State that allows Hestmor to stay still
/// </summary>
public partial class Idle : PlayerState
{
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
			StateMachine.ChangeState("Melee");
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

	internal override void OnEnter()
	{
		Player.CanChangeFacingDirection = true;
		Player.Velocity = new Vector2(0f, 0f);

		AnimPlayer.Play("idle");
	}

	internal override void PhysicsUpdate(double delta)
	{
		if(!Player.IsOnFloor())
		{
			StateMachine.ChangeState("Fall");
			return;
		}

		var movement = Input.GetAxis("move_left", "move_right");

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
