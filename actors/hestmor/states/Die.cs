using Epilogue.Global.Singletons;
using Epilogue.Nodes;
using Godot;

namespace Epilogue.Actors.Hestmor.States;
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
		_player = (Player)stateMachine.Owner;
		_playerEvents = playerEvents;

		SpriteSheetId = (int)Enums.SpriteSheetId.Bob;
	}

	internal override void OnEnter(params object[] args)
	{
		_player.HurtBox.CanRecoverFromDamage = false;
		_player.CanChangeFacingDirection = false;

		_playerEvents.EmitSignal(PlayerEvents.SignalName.PlayerIsDying);

		AnimPlayer.Play("Combat/die");
		AnimPlayer.AnimationFinished += (StringName animationName) => _playerEvents.EmitSignal(PlayerEvents.SignalName.PlayerDied);
	}
}
