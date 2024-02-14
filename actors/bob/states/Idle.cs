using Epilogue.Actors.Hestmor.Enums;
using Epilogue.Const;
using Epilogue.Nodes;
using Godot;

namespace Epilogue.Actors.Hestmor.States;
/// <inheritdoc/>
public partial class Idle : State
{
	private readonly float _sleepDelay;
	private readonly Player _player;

	private float _sleepTimer;

	/// <summary>
	/// 	State that allows Hestmor to stay still
	/// </summary>
	/// <param name="stateMachine">The State Machine who owns this State</param>
	/// <param name="sleepDelay">The time (in seconds) it takes for Hestmor to start sleeping</param>
	public Idle(StateMachine stateMachine, float sleepDelay) : base(stateMachine)
	{
		_sleepDelay = sleepDelay;
		_player = (Player)stateMachine.Owner;
	}

	internal override void OnInput(InputEvent @event)
	{
		if (Input.IsActionJustPressed("jump"))
		{
			if (!_player.RayCasts["Head"].IsColliding() && _player.RayCasts["Feet"].IsColliding())
			{
				var raycast = _player.RayCasts["Ledge"];
				var originalPosition = raycast.Position;

				raycast.Position = new Vector2(0f, -Const.Constants.MAP_TILE_SIZE - 1);

				raycast.ForceRaycastUpdate();

				if (!raycast.IsColliding())
				{
					StateMachine.ChangeState(typeof(Vault));
				}

				raycast.Position = originalPosition;
			}
			else
			{
				StateMachine.ChangeState(typeof(Jump), StateType.StandingJump);
			}
		}
		else if (Input.IsActionJustPressed("crouch"))
		{
			StateMachine.ChangeState(typeof(Crouch));
		}
		else if (Input.IsActionJustPressed("melee"))
		{
			StateMachine.ChangeState(typeof(MeleeAttack), StateType.SwipeAttack);
		}
		else if (Input.IsActionJustPressed("slide"))
		{
			StateMachine.ChangeState(typeof(Slide), StateType.FrontRoll);
		}
		else if (Input.IsActionJustPressed("look_up"))
		{
			StateMachine.ChangeState(typeof(LookUp));
		}
		else if (Input.IsActionJustPressed("growl"))
		{
			StateMachine.ChangeState(typeof(Growl));
		}
	}

	internal override void OnEnter(params object[] args)
	{
		_sleepTimer = 0f;

		_player.CanChangeFacingDirection = true;

		AnimPlayer.Play("idle");
	}

	internal override void PhysicsUpdate(double delta)
	{
		_sleepTimer += (float)delta;

		if (_sleepTimer >= _sleepDelay)
		{
			StateMachine.ChangeState(typeof(Sleep));
			return;
		}

		if (!_player.IsOnFloor())
		{
			StateMachine.ChangeState(typeof(Fall), StateType.LongJump);
			return;
		}

		var movement = Input.GetAxis("move_left", "move_right");

		if (movement != 0f)
		{
			if (_player.IsOnWall() && movement == -_player.GetWallNormal().X)
			{
				return;
			}

			StateMachine.ChangeState(_player.RunEnabled ? typeof(Run) : typeof(Walk));
			return;
		}

		if (Input.IsActionPressed("crouch"))
		{
			StateMachine.ChangeState(typeof(Crouch));
		}
	}
}
