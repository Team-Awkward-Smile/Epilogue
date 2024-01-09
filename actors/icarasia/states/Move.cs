using Epilogue.Actors.Icarasia.Enums;
using Epilogue.Const;
using Epilogue.Nodes;
using Godot;

namespace Epilogue.Actors.Icarasia.States;
/// <inheritdoc/>
public partial class Move : State
{
	private readonly Icarasia _icarasia;
	private readonly Player _player;
	private readonly float _moveSpeed;
	private readonly float _shotDesiredDistance;

	/// <summary>
	///		State used by the Icarasia when moving towards the player
	/// </summary>
	/// <param name="stateMachine">State Machine who owns this State</param>
	/// <param name="player">Reference to the player character</param>
	/// <param name="moveSpeed">Speed (in units) the Icarasia will move</param>
	/// <param name="shotDesiredDistance">Distance (in units) the Icarasia will try to get from the player before attacking</param>
	public Move(StateMachine stateMachine, Player player, float moveSpeed, float shotDesiredDistance) : base(stateMachine)
	{
		_icarasia = (Icarasia)stateMachine.Owner;
		_player = player;
		_moveSpeed = moveSpeed;
		_shotDesiredDistance = shotDesiredDistance;
	}

	internal override void OnEnter(params object[] args)
	{
		AnimPlayer.Play("move");
	}

	internal override void PhysicsUpdate(double delta)
	{
		var desiredDistance = _icarasia.PreferredAttack == PreferredAttack.Projectile ? _shotDesiredDistance : 35f;
		var targetPosition = _player.GlobalPosition - new Vector2(0f, Const.Constants.PLAYER_HEIGHT / 2);

		_icarasia.Velocity = _icarasia.DistanceToPlayer > desiredDistance
			? (targetPosition - _icarasia.GlobalPosition).Normalized() * _moveSpeed
			: _icarasia.ShotAngle == null ? new Vector2(0f, targetPosition.Y > _icarasia.GlobalPosition.Y ? _moveSpeed : -_moveSpeed) : Vector2.Zero;

		if (_icarasia.Velocity.X != 0f)
		{
			_icarasia.TurnTowards(_player);
		}

		_ = _icarasia.MoveAndSlide();

		if (_icarasia.PreferredAttack == PreferredAttack.Projectile && _icarasia.AttackTimer >= _icarasia.ShotCooldown && _icarasia.ShotAngle is not null)
		{
			StateMachine.ChangeState(typeof(Shoot), _icarasia.ShotAngle);
		}
		else if (_icarasia.AttackTimer >= _icarasia.StingerCooldown && _icarasia.StingDirection is not null)
		{
			StateMachine.ChangeState(typeof(Sting), _icarasia.StingDirection);
		}
	}
}
