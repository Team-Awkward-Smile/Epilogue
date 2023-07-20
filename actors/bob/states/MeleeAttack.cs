using Epilogue.global.enums;
using Epilogue.global.singletons;
using Epilogue.nodes;
using Godot;

namespace Epilogue.actors.hestmor.states;
public partial class MeleeAttack : StateComponent
{
	private CollisionShape2D _hitbox;
	private Events _eventsSingleton;
	private Actor _enemy;

	public override void OnEnter()
	{
		// The attack audio is controlled by the animation

		EmitSignal(SignalName.StateStarted);

		StateMachine.CanInteract = false;

		if(Actor.RayCasts["Enemy"].IsColliding())
		{
			_enemy = (Actor) Actor.RayCasts["Enemy"].GetCollider();

			if(((NPCHealth) _enemy.Health).IsVulnerable)
			{
				Actor.CanChangeFacingDirection = false;

				_eventsSingleton = GetNode<Events>("/root/Events");

				_eventsSingleton.EmitGlobalSignal("StateAwaitingForGloryKillInput");
				_eventsSingleton.GloryKillInputReceived += PerformExecution;

				return;
			}
		}

		AnimPlayer.Play("melee_attack");
		AnimPlayer.AnimationFinished += FinishAttack;
	}

	public async void PerformExecution(GloryKillSpeed speed)
	{
		_eventsSingleton.GloryKillInputReceived -= PerformExecution;

		var animation = "glory_kill_" + speed switch
		{
			GloryKillSpeed.Slow => "slow",
			_ => "fast"
		};

		AnimPlayer.Play(animation);

		await ToSignal(AnimPlayer, "animation_finished");

		// TODO: 68 - temporary solution, we need to think if this can break in the future
		_enemy.Health.DealDamage(100);

		StateMachine.ChangeState("Idle");
	}

	private void FinishAttack(StringName animName)
	{
		AnimPlayer.AnimationFinished -= FinishAttack;

		StateMachine.ChangeState("Idle");
	}

	public override void OnLeave()
	{
		StateMachine.CanInteract = true;
	}
}
