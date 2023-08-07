using Epilogue.global.enums;
using Epilogue.global.singletons;
using Epilogue.nodes;
using Godot;

namespace Epilogue.actors.hestmor.states;
/// <summary>
///		State that allows Hestmor to perform melee attacks and Executions
/// </summary>
public partial class MeleeAttack : PlayerState
{
	private CollisionShape2D _hitbox;
	private PlayerEvents _eventsSingleton;
	private Npc _enemy;

	internal override void OnEnter()
	{
		// The attack audio is controlled by the animation

		StateMachine.CanInteract = false;

		if(Player.RayCasts["Enemy"].IsColliding())
		{
			_enemy = (Npc) Player.RayCasts["Enemy"].GetCollider();

			if(_enemy.IsVulnerable)
			{
				Player.CanChangeFacingDirection = false;

				_eventsSingleton = GetNode<PlayerEvents>("/root/PlayerEvents");

				_eventsSingleton.EmitGlobalSignal("StateAwaitingForExecutionSpeed");
				_eventsSingleton.ExecutionSpeedSelected += PerformExecution;

				return;
			}
		}

		AnimPlayer.Play("melee_attack");
		AnimPlayer.AnimationFinished += FinishAttack;
	}

	private async void PerformExecution(ExecutionSpeed speed)
	{
		_eventsSingleton.ExecutionSpeedSelected -= PerformExecution;

		var animation = "glory_kill_" + speed switch
		{
			ExecutionSpeed.Slow => "slow",
			_ => "fast"
		};

		AnimPlayer.Play(animation);

		await ToSignal(AnimPlayer, "animation_finished");

		// TODO: 68 - temporary solution, we need to think if this can break in the future
		_enemy.DealDamage(100);

		StateMachine.ChangeState("Idle");
	}

	private void FinishAttack(StringName animName)
	{
		AnimPlayer.AnimationFinished -= FinishAttack;

		StateMachine.ChangeState("Idle");
	}

	internal override void OnLeave()
	{
		StateMachine.CanInteract = true;
	}
}
