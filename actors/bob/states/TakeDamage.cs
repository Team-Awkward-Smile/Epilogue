using Epilogue.nodes;

using Godot;

namespace Epilogue.actors.hestmor.states;
/// <summary>
///		State that allows Hestmor to react to damage taken
/// </summary>
public partial class TakeDamage : PlayerState
{
	internal override void OnEnter()
	{
		Player.CanChangeFacingDirection = false;

		AnimPlayer.Play("Combat/take_damage");
		AnimPlayer.AnimationFinished += FinishAnimation;
	}

	private void FinishAnimation(StringName animationName)
	{
		AnimPlayer.AnimationFinished -= FinishAnimation;
		StateMachine.ChangeState("Idle");
	}
}
