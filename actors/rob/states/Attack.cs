using Epilogue.nodes;

using Godot;

public partial class Attack : NpcState
{
	internal override void OnEnter(params object[] args)
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

	public void SpawnHitBox()
	{
		var area = GetNode<HitBox>("../../FlipRoot/HitBox");

		area.Damage = 1f;
		area.CollisionShape = GD.Load<CircleShape2D>("res://actors/rob/hitboxes/slam.tres");
	}

	public void DestroyHitBox()
	{
		var area = GetNode<HitBox>("../../FlipRoot/HitBox");

		area.DeleteHitBox();
	}
}
