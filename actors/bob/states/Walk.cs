using Epilogue.Actors.Hestmor.Enums;
using Epilogue.Global.Enums;
using Epilogue.Nodes;
using Godot;

namespace Epilogue.Actors.Hestmor.States;
/// <inheritdoc/>
public partial class Walk : State
{
	private static Vector2 s_footstepManagerPositivePosition = new(8f, 1f);
	private static Vector2 s_footstepManagerNegativePosition = new(-8f, 1f);

	private readonly float _walkSpeed;
	private readonly Player _player;
	private readonly FootstepManager _footstepManager;

	/// <summary>
	/// 	State that allows Hestmor to walk
	/// </summary>
	/// <param name="stateMachine">The State Machine who owns this State</param>
	/// <param name="walkSpeed">The horizontal speed of Hestmor when Walking</param>
	public Walk(StateMachine stateMachine, float walkSpeed) : base(stateMachine)
	{
		_walkSpeed = walkSpeed;
		_player = (Player)stateMachine.Owner;
		_footstepManager = _player.GetNode<FootstepManager>("FlipRoot/ActorAudioPlayer/FootstepManager");
	}

	internal override void OnInput(InputEvent @event)
	{
		if (@event.IsActionPressed("jump"))
		{
			if (_player.RayCasts["Head"].IsColliding() && !_player.RayCasts["Ledge"].IsColliding())
			{
				StateMachine.ChangeState(typeof(GrabLedge));
			}
			else
			{
				StateMachine.ChangeState(typeof(Jump), StateType.LowJump);
			}
		}
		else if (@event.IsActionPressed("melee"))
		{
			StateMachine.ChangeState(typeof(MeleeAttack), StateType.UppercutPunch);
		}
		else if (@event.IsActionPressed("crouch"))
		{
			StateMachine.ChangeState(typeof(Crouch));
		}
		else if (@event.IsActionPressed("slide"))
		{
			StateMachine.ChangeState(typeof(Slide), StateType.KneeSlide);
		}
	}

	internal override void OnEnter(params object[] args)
	{
		AnimPlayer.Play("walk");

		_player.CanChangeFacingDirection = true;
		_player.RotationDegrees = 0f;
		_footstepManager.Position = s_footstepManagerPositivePosition;
	}

	internal override void PhysicsUpdate(double delta)
	{
		var movementDirection = Input.GetAxis("move_left", "move_right");

		if (movementDirection != 0f)
		{
			movementDirection = movementDirection > 0 ? 1 : -1;

			var velocity = _player.Velocity;

			velocity.Y += StateMachine.Gravity * (float)delta;
			velocity.X = movementDirection * _walkSpeed * (float)delta * 60f;

			if ((movementDirection > 0 && _player.FacingDirection == ActorFacingDirection.Left) ||
				(movementDirection < 0 && _player.FacingDirection == ActorFacingDirection.Right))
			{
				velocity.X /= 2;

				_footstepManager.Position = s_footstepManagerNegativePosition;
			}

			_player.Velocity = velocity;
		}

		_player.MoveAndSlide();

		//var floorNormal = _player.GetFloorNormal();
		//var goingDownSlope = (movementDirection < 0 && floorNormal.X < 0) || (movementDirection > 0 && floorNormal.X > 0);

		if (movementDirection == 0f || _player.IsOnWall())
		{
			StateMachine.ChangeState(typeof(Idle));
		}
		else if (!_player.IsOnFloor())
		{
			StateMachine.ChangeState(typeof(Fall), StateType.LongJump);
		}
		else if (_player.RunEnabled)
		{
			StateMachine.ChangeState(typeof(Run));
		}
	}
}
