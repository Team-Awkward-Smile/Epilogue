using Epilogue.Global.Enums;
using Epilogue.Nodes;
using Godot;

namespace Epilogue.Actors.Icarasia.States;
/// <inheritdoc/>
public partial class Flee : State
{
	private readonly Icarasia _icarasia;
	private readonly Player _player;

	private float _moveSpeed;
	private float _fleeDuration;
	private float _timer;

	/// <summary>
	///		State used by the Icarasia when fleeing from the player
	/// </summary>
	/// <param name="stateMachine">State Machine who owns this State</param>
	/// <param name="player">Reference to the Player character</param>
	public Flee(StateMachine stateMachine, Player player) : base(stateMachine)
	{
		_icarasia = (Icarasia)stateMachine.Owner;
		_player = player;
	}

	// args[0] - float - Movement Speed
	// args[1] - float - Flee Duration
	internal override void OnEnter(params object[] args)
	{
		_moveSpeed = (float)args[0];
		_fleeDuration = (float)args[1];
		_timer = 0f;
	}

	internal override void PhysicsUpdate(double delta)
	{
		_timer += (float)delta;

		if (_timer >= _fleeDuration)
		{
			StateMachine.ChangeState(typeof(Move));

			return;
		}

		_icarasia.Velocity = (_player.GlobalPosition - new Vector2(0f, Constants.Constants.PLAYER_HEIGHT / 2) - _icarasia.GlobalPosition).Normalized() * _moveSpeed * -1;

		if (_icarasia.Velocity.X != 0f)
		{
			_icarasia.SetFacingDirection(_icarasia.Velocity.X > 0f ? ActorFacingDirection.Right : ActorFacingDirection.Left);
		}

		_ = _icarasia.MoveAndSlide();
	}
}
