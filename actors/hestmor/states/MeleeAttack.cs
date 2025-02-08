using Epilogue.Actors.Hestmor.Enums;
using Epilogue.Global.Enums;
using Epilogue.Global.Singletons;
using Epilogue.Nodes;
using Godot;
using System.Threading.Tasks;

namespace Epilogue.Actors.Hestmor.States;
/// <inheritdoc/>
public partial class MeleeAttack : State
{
	private readonly float _RunAttackSpeed;
	private readonly Player _player;

	private PlayerEvents _eventsSingleton;
	private Npc _enemy;
	private StateType _attackType;

	/// <summary>
	/// 	State that allows Hestmor to perform melee attacks and Executions
	/// </summary>
	/// <param name="stateMachine">The State Machine who owns this State</param>
	/// <param name="RunAttackSpeed">The horizontal speed of Hestmor when performing a Slide Attack</param>
	public MeleeAttack(StateMachine stateMachine, float RunAttackSpeed) : base(stateMachine)
	{
		_RunAttackSpeed = RunAttackSpeed;
		_player = (Player)stateMachine.Owner;

		SpriteSheetId = (int)Enums.SpriteSheetId.Bob;
	}

	internal override void OnStateMachineActivation()
	{
		_eventsSingleton = StateMachine.GetNode<PlayerEvents>("/root/PlayerEvents");

		_eventsSingleton.ExecutionSpeedSelected += (ExecutionSpeed speed) =>
		{
			if (!Active)
			{
				return;
			}

			PerformExecution(speed);
		};

		AnimPlayer.AnimationFinished += (StringName animationName) =>
		{
			if (!Active || !animationName.ToString().StartsWith("Combat"))
			{
				return;
			}

			StateMachine.ChangeState(typeof(Idle));
		};
	}

	internal override void OnEnter(params object[] args)
	{
		// The attack audio is controlled by the animation

		_player.CanChangeFacingDirection = false;
		_player.CanInteract = false;
		_player.Velocity = Vector2.Zero;

		_attackType = (StateType)args[0];

		if (_player.HoldingSword)
		{
			AnimPlayer.Play("Combat/sword_slash");
		}
		else
		{
			var animation = _attackType switch
			{
				StateType.RunAttack => "run_attack_melee",
				StateType.UppercutPunch => "uppercut_punch",
				_ => "melee_attack",
			};

			AnimPlayer.Play($"Combat/{animation}");

			if (_attackType == StateType.RunAttack)
			{
				_player.Velocity = new Vector2(_RunAttackSpeed * (_player.FacingDirection == ActorFacingDirection.Left ? -1 : 1), 0f);
			}
		}
	}

	private async void PerformExecution(ExecutionSpeed speed)
	{
		var animation = "Combat/execution_" + speed switch
		{
			ExecutionSpeed.Slow => "slow",
			_ => "fast"
		};

		AnimPlayer.Play(animation);

		await StateMachine.ToSignal(AnimPlayer, AnimationMixer.SignalName.AnimationFinished);

		_enemy.Execute(speed);
	}

	internal override void PhysicsUpdate(double delta)
	{
		if (_attackType != StateType.RunAttack)
		{
			return;
		}

		_player.MoveAndSlide();
	}

	internal override Task OnLeave()
	{
		_player.CanInteract = true;

		return Task.CompletedTask;
	}
}