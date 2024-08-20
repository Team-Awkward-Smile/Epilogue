using Epilogue.Actors.Hestmor.Enums;
using Epilogue.Nodes;
using Godot;
using System.Runtime.InteropServices;

namespace Epilogue.Actors.Hestmor.States;
/// <inheritdoc/>
public partial class Idle : State
{
	private readonly float _sleepDelay;
	private readonly Player _player;
	private readonly FootstepManager _footstepManager;

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
		_footstepManager = _player.GetNode<FootstepManager>("FlipRoot/ActorAudioPlayer/FootstepManager");

		SpriteSheetId = (int)Enums.SpriteSheetId.IdleWalk;
	}


	// I've wrote 'AudioPlayer.Stop("generic");' multiple times because
	// I couldn't find the actuel way to implemented properly
	// It was requested that the idle sound should ONLY play when in idle state
	// so the sound stops whenever it exits the idle state
	internal override void OnInput(InputEvent @event)
	{
		_sleepTimer = 0f;

		if (@event.IsActionPressed("jump"))
		{
			if (!_player.RayCasts["Head"].IsColliding() && _player.RayCasts["Feet"].IsColliding())
			{
				var raycast = _player.RayCasts["Ledge"];
				var originalPosition = raycast.Position;

				raycast.Position = new Vector2(0f, -Const.Constants.MAP_TILE_SIZE - 1);

				raycast.ForceRaycastUpdate();

				if (!raycast.IsColliding())
				{
					AudioPlayer.Stop("generic");
					StateMachine.ChangeState(typeof(Vault));
				}

				raycast.Position = originalPosition;
			}
			else
			{
				AudioPlayer.Stop("generic");
				StateMachine.ChangeState(typeof(Jump), StateType.StandingJump);
			}
		}
		else if (@event.IsActionPressed("crouch"))
		{
			AudioPlayer.Stop("generic");
			StateMachine.ChangeState(typeof(Crouch));
		}
		else if (@event.IsActionPressed("melee"))
		{
			AudioPlayer.Stop("generic");
			StateMachine.ChangeState(typeof(MeleeAttack), StateType.SwipeAttack);
		}
		else if (@event.IsActionPressed("slide"))
		{
			AudioPlayer.Stop("generic");
			StateMachine.ChangeState(typeof(Slide), StateType.FrontRoll);
		}
		else if (@event.IsActionPressed("growl"))
		{
			AudioPlayer.Stop("generic");
			StateMachine.ChangeState(typeof(Growl));
		}
	}

	internal override void OnEnter(params object[] args)
	{
		_sleepTimer = 0f;

		_player.CanChangeFacingDirection = true;
		_footstepManager.Position = new(0f, 1f);

		AnimPlayer.Play("idle");
		AudioPlayer.PlayGenericSfx("Idle");
		
	}


	internal override void PhysicsUpdate(double delta)
	{
		_sleepTimer += (float)delta;

		if (_sleepTimer >= _sleepDelay)
		{
			AudioPlayer.Stop("generic");
			StateMachine.ChangeState(typeof(Sleep));
			return;
		}

		if (!_player.IsOnFloor())
		{
			AudioPlayer.Stop("generic");
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

			AudioPlayer.Stop("generic");
			StateMachine.ChangeState(_player.RunEnabled ? typeof(Run) : typeof(Walk));
			return;
		}
	}

	
}
