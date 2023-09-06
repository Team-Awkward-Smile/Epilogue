using Epilogue.nodes;

using Godot;

public partial class Attack : NpcState
{
	internal override void OnEnter()
	{
		Npc.CanChangeFacingDirection = false;

		AnimPlayer.PlayBackwards("Combat/attack");
		AnimPlayer.AnimationFinished += OnAnimationFinished;
	}

	private void OnAnimationFinished(StringName animName)
	{
		AnimPlayer.AnimationFinished -= OnAnimationFinished;
		StateMachine.ChangeState("Move");
	}

	internal override void OnLeave()
	{
		Npc.CanChangeFacingDirection = true;
	}
}
