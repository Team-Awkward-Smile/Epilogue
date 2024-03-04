using Epilogue.Actors.Hestmor.Enums;
using Epilogue.Global.Enums;
using Epilogue.Nodes;

using Godot;

namespace Epilogue.Actors.Hestmor.States;
/// <inheritdoc/>
public partial class Run : State
{
	private readonly float _runSpeed;
	private readonly Player _player;

	/// <summary>
	/// 	State that allows Hestmor to run
	/// </summary>
	/// <param name="stateMachine">The State Machine who owns this State</param>
	/// <param name="runSpeed">The horizontal speed of Hestmor when Running</param>
	public Run(StateMachine stateMachine, float runSpeed) : base(stateMachine)
	{
		_player = (Player)stateMachine.Owner;
		_runSpeed = runSpeed;
	}

	internal override void OnInput(InputEvent @event)
	{
		if (@event.IsActionPressed("jump"))
		{
			if (_player.IsOnWall())
			{
				if (_player.RayCasts["Head"].IsColliding())
				{
					// Is near a wall
					StateMachine.ChangeState(typeof(Jump), StateType.LongJump);
				}
				else
				{
					// Is near a ledge
					StateMachine.ChangeState(typeof(GrabLedge));
				}
			}
			else
			{
				StateMachine.ChangeState(typeof(Jump), StateType.LongJump);
			}
		}
		else if (@event.IsActionPressed("melee"))
		{
			StateMachine.ChangeState(typeof(MeleeAttack), StateType.SlideAttack);
		}
		else if (@event.IsActionPressed("slide"))
		{
			StateMachine.ChangeState(typeof(Slide), StateType.LongSlide);
		}
	}

	internal override void OnEnter(params object[] args)
	{
		AnimPlayer.Play("walk", -1, 2f);

		_player.CanChangeFacingDirection = true;
		_player.RotationDegrees = 0f;
	}

	internal override void PhysicsUpdate(double delta)
	{
		var movementDirection = Input.GetAxis("move_left", "move_right");

		if (movementDirection != 0f)
		{
			movementDirection = movementDirection > 0 ? 1 : -1;

			var velocity = _player.Velocity;

			velocity.Y += StateMachine.Gravity * (float)delta;
			velocity.X = movementDirection * _runSpeed;

			if ((movementDirection > 0 && _player.FacingDirection == ActorFacingDirection.Left) ||
				(movementDirection < 0 && _player.FacingDirection == ActorFacingDirection.Right))
			{
				velocity.X /= 2;
			}

			_player.Velocity = velocity;
		}

		_player.MoveAndSlide();

		var floorNormal = _player.GetFloorNormal();
		var goingDownSlope = (movementDirection < 0 && floorNormal.X < 0) || (movementDirection > 0 && floorNormal.X > 0);

		if (movementDirection == 0f || _player.IsOnWall())
		{
			StateMachine.ChangeState(typeof(Idle));
		}
		else if (!_player.IsOnFloor())
		{
			StateMachine.ChangeState(typeof(Fall), StateType.LongJump);
		}
		else if (!_player.RunEnabled)
		{
			StateMachine.ChangeState(typeof(Walk));
		}
		else if (_player.RotationDegrees >= 40f && !goingDownSlope)
		{
			StateMachine.ChangeState(typeof(Crawl));
		}
	}
}
