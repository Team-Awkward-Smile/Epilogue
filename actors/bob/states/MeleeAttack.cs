using Epilogue.nodes;
using Godot;

namespace Epilogue.actors.hestmor.states;
public partial class MeleeAttack : StateComponent
{
	private CollisionShape2D _hitbox;

	public override void OnEnter()
	{
		// The attack audio is controlled by the animation

		EmitSignal(SignalName.StateStarted);

		if(Actor.RayCasts["Enemy"].IsColliding())
		{
			var enemy = (Actor) Actor.RayCasts["Enemy"].GetCollider();

			if(enemy.Health.IsInGloryKillMode)
			{
				AnimPlayer.Play("glory_kill");

				enemy.Health.GloryKill();

				GetTree().CreateTimer(1f).Timeout += () => StateMachine.ChangeState("Idle");

				return;
			}
		}

		var hitboxShape = (CircleShape2D) GD.Load("res://actors/bob/hitboxes/melee_1.tres");

		_hitbox = (CollisionShape2D) HitBoxContainer.GetChild(0);
		_hitbox.Shape = hitboxShape;
		HitBoxContainer.AreaEntered += DealDamage;

		AnimPlayer.Play("melee_attack");
		AnimPlayer.AnimationFinished += FinishAttack;
	}

	private void FinishAttack(StringName animName)
	{
		AnimPlayer.AnimationFinished -= FinishAttack;
		HitBoxContainer.AreaEntered -= DealDamage;
		StateMachine.ChangeState("Idle");
	}

	private void DealDamage(Area2D hurtbox)
	{
		var enemy = (Actor) hurtbox.Owner;

		enemy.Health.ApplyDamage(1);
	}

	public override void OnLeave()
	{
		_hitbox.Shape = null;

		EmitSignal(SignalName.StateFinished);
	}
}
