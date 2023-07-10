using Epilogue.nodes;
using Godot;

namespace Epilogue.actors.hestmor.states;
public partial class MeleeAttack : PlayerState
{
	CollisionShape2D _hitbox;

	public override void OnEnter()
	{
		// The attack audio is controlled by the animation

		var hitboxShape = (CircleShape2D) GD.Load("res://actors/bob/hitboxes/melee_1.tres");

		_hitbox = (CollisionShape2D) HitBoxContainer.GetChild(0);
		_hitbox.Shape = hitboxShape;

		AnimPlayer.Play("melee_attack");
		AnimPlayer.AnimationFinished += FinishAttack;
	}

	private void FinishAttack(StringName animName)
	{
		AnimPlayer.AnimationFinished -= FinishAttack;
		StateMachine.ChangeState("Idle");
	}

	public override void OnLeave()
	{
		_hitbox.Shape = null;
	}
}
