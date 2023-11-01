using Epilogue.actors.hestmor.enums;
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
	[Export] private float _slideAttackSpeed = 150f;

	private CollisionShape2D _hitbox;
	private PlayerEvents _eventsSingleton;
	private Npc _enemy;
	private StateType _attackType;

	internal override void OnEnter(params object[] args)
	{
		// The attack audio is controlled by the animation

		Player.CanChangeFacingDirection = false;

		_attackType = (StateType) args[0];

		var label = Player.GetNode<Label>("temp_StateName");

		label.Text = _attackType.ToString();
		label.Show();

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

		if(Player.HoldingSword)
		{
			AnimPlayer.Play("Combat/sword_slash");
		}
		else
		{
			var animation = _attackType switch
			{
				StateType.SlideAttack => "slide_attack",
				_ => "melee_attack",
			};

			AnimPlayer.Play($"Combat/{animation}");

			if(_attackType == StateType.SlideAttack)
			{
				Player.Velocity = new Vector2(_slideAttackSpeed * (Player.FacingDirection == ActorFacingDirection.Left ? -1 : 1), 0f);
			}
		}

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

	internal override void PhysicsUpdate(double delta)
	{
		if(_attackType != StateType.SlideAttack)
		{
			return;
		}

		Player.MoveAndSlideWithRotation();
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
		Player.GetNode<Label>("temp_StateName").Hide();
	}
}
