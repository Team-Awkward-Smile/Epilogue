using Epilogue.Nodes;

using Godot;

namespace Epilogue.Actors.Hestmor.States;
/// <inheritdoc/>
public partial class TakeDamage : State
{
	private readonly Player _player;

	/// <summary>
	/// 	State that allows Hestmor to react to damage taken
	/// </summary>
	/// <param name="stateMachine">The State Machine who owns this State</param>
	public TakeDamage(StateMachine stateMachine) : base(stateMachine)
	{
		_player = (Player)stateMachine.Owner;
	}

	internal override void OnEnter(params object[] args)
	{
		_player.CanChangeFacingDirection = false;

		AnimPlayer.Play("Combat/take_damage");
		AnimPlayer.AnimationFinished += FinishAnimation;
	}

	private void FinishAnimation(StringName animationName)
	{
		AnimPlayer.AnimationFinished -= FinishAnimation;
		StateMachine.ChangeState(typeof(Idle));
	}
}
