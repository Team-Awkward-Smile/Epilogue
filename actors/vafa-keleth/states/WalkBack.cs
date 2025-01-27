using Epilogue.Extensions;
using Epilogue.Nodes;
using Godot;

namespace Epilogue.Actors.VafaKeleth.States;
/// <inheritdoc/>
public partial class WalkBack : State
{
	private readonly VafaKeleth _vafaKeleth;
	private readonly Player _player;
	private readonly float _moveSpeed;

	private float _exitTimer;

	/// <summary>
	///		State that allows the Vafa'Keleth to move away from the player while facing them
	/// </summary>
	/// <param name="stateMachine">State Machine that owns this State</param>
	/// <param name="player">Player character</param>
	/// <param name="moveSpeed">Speed (in units) of the movement</param>
	public WalkBack(StateMachine stateMachine, Player player, float moveSpeed) : base(stateMachine)
	{
		_vafaKeleth = (VafaKeleth)stateMachine.Owner;
		_player = player;
		_moveSpeed = moveSpeed;
	}

	// args[0] - float - Time (in seconds) this State will remain active before changing to another one
	internal override void OnEnter(params object[] args)
	{
		_vafaKeleth.CanAttemptSlide = true;

		_exitTimer = (float)args[0];
	}

	internal override void PhysicsUpdate(double delta)
	{
		if ((_exitTimer -= (float)delta) <= 0)
		{
			StateMachine.ChangeState(typeof(Combat));

			return;
		}

		_vafaKeleth.Velocity = _vafaKeleth.PlayerNavigationAgent2D.GetNextVelocity(_vafaKeleth.GlobalPosition, _moveSpeed) * new Vector2(-1f, 0f);

		_vafaKeleth.TurnTowards(_player);
		_vafaKeleth.MoveAndSlide();
	}
}
