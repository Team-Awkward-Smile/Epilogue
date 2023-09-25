using Epilogue.nodes;

using Godot;
using System;

namespace Epilogue.actors.hestmor.states;
public partial class TakeDamage : PlayerState
{
	internal override void OnEnter(params object[] args)
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
