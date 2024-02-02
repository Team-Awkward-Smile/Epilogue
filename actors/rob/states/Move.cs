using Epilogue.Extensions;
using Epilogue.Global.Enums;
using Epilogue.Nodes;
using Godot;

namespace Epilogue.Actors.rob.states;
/// <inheritdoc/>
public partial class Move : State
{
	private readonly Rob _rob;
	private readonly Player _player;
	private readonly float _moveSpeed;

	private bool _isIdle = false;

	/// <summary>
	/// 	State that allows Rob to move around the map, trying to reach the player
	/// </summary>
	/// <param name="stateMachine">The State Machine who owns this State</param>
	/// <param name="player">A reference to the player character</param>
	/// <param name="moveSpeed">Horizontal speed of Rob when walking towards Hestmor</param>
	public Move(StateMachine stateMachine, Player player, float moveSpeed) : base(stateMachine)
	{
		_rob = (Rob) stateMachine.Owner;
		_player = player;
		_moveSpeed = moveSpeed;
	}

	internal override void OnEnter(params object[] args)
	{
		_rob.CanChangeFacingDirection = true;

		AnimPlayer.PlayBackwards("walk");
	}

	internal override void PhysicsUpdate(double delta)
	{
		var distance = _rob.PlayerNavigationAgent2D.DistanceToTarget();
		var velocity = Vector2.Zero;
		var finalSpeed = _moveSpeed * _rob.SpeedMultiplier;

		if(!_rob.IsPlayerReachable)
		{
			StateMachine.ChangeState(typeof(Wander));

			return;
		}
		else if(distance > (_rob.CanShoot ? 200f : 30f))
		{
			velocity = _rob.PlayerNavigationAgent2D.GetNextVelocity(_rob.GlobalPosition, finalSpeed);
		}
		else if(_rob.CanShoot && distance <= 150f)
		{
			velocity = _rob.PlayerNavigationAgent2D.GetNextVelocity(_rob.GlobalPosition, finalSpeed) * -1;
		}
		else
		{
			var angle = Mathf.RadToDeg(Mathf.Atan2(_player.GlobalPosition.Y - _rob.GlobalPosition.Y, _player.GlobalPosition.X - _rob.GlobalPosition.X));
			var shotAimed = Mathf.Abs(angle) switch
			{
				<= 2 or >= 178 => true,
				_ => false
			};

			if(_rob.CanShoot && shotAimed && _rob.AttackTimer >= _rob.ShotCooldown)
			{
				StateMachine.ChangeState(typeof(Shoot));
				
				_rob.AttackTimer = 0f;

				return;
			}
			else if(!_rob.CanShoot && _rob.AttackTimer >= _rob.AttackTimer)
			{
				StateMachine.ChangeState(typeof(Attack));

				_rob.AttackTimer = 0f;

				return;
			}
		}

		_rob.Velocity = new Vector2(velocity.X, velocity.Y + StateMachine.Gravity * (float) delta);
		_rob.MoveAndSlide();

		if(_rob.Velocity.X != 0)
		{
			_rob.SetFacingDirection(_rob.Velocity.X > 0 ? ActorFacingDirection.Right : ActorFacingDirection.Left);

			if(_isIdle)
			{
				AnimPlayer.PlayBackwards("walk");
				
				_isIdle = false;
			}
		}
		else
		{
			_rob.TurnTowards(_player);

			AnimPlayer.PlayBackwards("idle");
			
			_isIdle = true;
		}
	}
}
