using Epilogue.Global.Enums;
using Epilogue.Nodes;
using Godot;

namespace Epilogue.Actors.Icarasia.States;
/// <inheritdoc/>
public partial class Charge : State
{
	private readonly Icarasia _icarasia;
	private readonly Player _player;
	private readonly float _chargeSpeed;
	private readonly float _chargeDuration;

	private Vector2 _targetPosition;
	private float _timer;
	private Vector2 _chargeDirection;

	/// <summary>
	///		State for the Icarasia to charge at the player.
	/// </summary>
	/// <param name="stateMachine">The State Machine who owns this State</param>
	/// <param name="player">Reference to the Player character</param>
	/// <param name="chargeSpeed">Speed (in units) the Icarasia will move while charging</param>
	/// <param name="chargeDuration">Time (in seconds) the charge will last</param>
	public Charge(StateMachine stateMachine, Player player, float chargeSpeed, float chargeDuration) : base(stateMachine)
	{
		_icarasia = (Icarasia)stateMachine.Owner;
		_player = player;
		_chargeSpeed = chargeSpeed;
		_chargeDuration = chargeDuration;
	}

	internal override void OnEnter(params object[] args)
	{
		_timer = 0f;
		_targetPosition = _player.GlobalPosition - new Vector2(0f, Const.Constants.PLAYER_HEIGHT / 2);
		_chargeDirection = (_targetPosition - _icarasia.GlobalPosition).Normalized() * _chargeSpeed;
		_icarasia.Velocity = _chargeDirection;

		_icarasia.TurnTowards(_player);
	}

	internal override void PhysicsUpdate(double delta)
	{
		_timer += (float)delta;

		if (_timer >= _chargeDuration)
		{
			StateMachine.ChangeState(typeof(Recover));
		}

		var offset = _icarasia.GlobalPosition.X - _player.GlobalPosition.X;

		if ((_icarasia.FacingDirection == ActorFacingDirection.Left && offset < -20) || (_icarasia.FacingDirection == ActorFacingDirection.Right && offset > 20))
		{
			StateMachine.ChangeState(typeof(Recover));
		}

		_ = _icarasia.MoveAndSlide();
	}
}
