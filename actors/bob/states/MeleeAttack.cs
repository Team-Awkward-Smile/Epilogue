using Epilogue.nodes;
using Godot;

namespace Epilogue.actors.hestmor.states;
public partial class MeleeAttack : StateComponent
{
	CollisionShape2D _hitbox;

	public override void OnEnter()
	{
		// The attack audio is controlled by the animation

		EmitSignal(SignalName.StateStarted);

		var hitboxShape = (CircleShape2D) GD.Load("res://actors/bob/hitboxes/melee_1.tres");

		_hitbox = (CollisionShape2D) HitBoxContainer.GetChild(0);
		_hitbox.Shape = hitboxShape;

		AnimPlayer.Play("Bob/melee_attack");
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

		EmitSignal(SignalName.StateFinished);
	}
}
