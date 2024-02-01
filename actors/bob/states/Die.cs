using Epilogue.Global.Singletons;
using Epilogue.Nodes;

using Godot;

namespace Epilogue.actors.hestmor.states;
/// <inheritdoc/>
public partial class Die : State
{
	private readonly Player _player;
	private readonly PlayerEvents _playerEvents;

	/// <summary>
	/// 	State that makes Hestmor die and trigger the approprate events
	/// </summary>
	/// <param name="stateMachine">The State Machine who owns this State</param>
	/// <param name="playerEvents">The Singleton that manages events related to the player</param>
	public Die(StateMachine stateMachine, PlayerEvents playerEvents) : base(stateMachine)
	{
		_player = (Player) stateMachine.Owner;
		_playerEvents = playerEvents;
	}

	internal override void OnEnter(params object[] args)
	{
		_player.HurtBox.SetDeferred("monitorable", false);
		_player.HurtBox.SetDeferred("monitoring", false);
		_player.CanChangeFacingDirection = false;

		AnimPlayer.Play("Combat/die");
		AnimPlayer.AnimationFinished += (StringName animationName) =>
		{
			_playerEvents.EmitGlobalSignal("PlayerDied");
		};
	}
}
