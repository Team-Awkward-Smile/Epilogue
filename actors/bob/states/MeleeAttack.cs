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
				Engine.TimeScale = 0.1f;
				Player.CanChangeFacingDirection = false;

				_eventsSingleton = GetNode<PlayerEvents>("/root/PlayerEvents");

				_eventsSingleton.EmitGlobalSignal("StateAwaitingForExecutionSpeed");
				_eventsSingleton.ExecutionSpeedSelected += PerformExecution;

				return;
			}
		}

		AnimPlayer.Play("Combat/melee_attack");
		AnimPlayer.AnimationFinished += FinishAttack;
	}

	private async void PerformExecution(ExecutionSpeed speed)
	{
		Engine.TimeScale = 1f;

		_eventsSingleton.ExecutionSpeedSelected -= PerformExecution;

		var animation = "Combat/execution_" + speed switch
		{
			ExecutionSpeed.Slow => "slow",
			_ => "fast"
		};

		AnimPlayer.Play(animation);

		await ToSignal(AnimPlayer, "animation_finished");

		_enemy.Execute(speed);

		StateMachine.ChangeState("Idle");
	}

	private void FinishAttack(StringName animName)
	{
		AnimPlayer.AnimationFinished -= FinishAttack;

		StateMachine.ChangeState("Idle");
	}

	internal override void OnLeave()
	{
		Engine.TimeScale = 1f;
		StateMachine.CanInteract = true;
	}
}
