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
	private readonly float _slideAttackSpeed;
	private readonly Player _player;

	private PlayerEvents _eventsSingleton;
	private Npc _enemy;
	private StateType _attackType;

	/// <summary>
	/// 	State that allows Hestmor to perform melee attacks and Executions
	/// </summary>
	/// <param name="stateMachine">The State Machine who owns this State</param>
	/// <param name="slideAttackSpeed">The horizontal speed of Hestmor when performing a Slide Attack</param>
	public MeleeAttack(StateMachine stateMachine, float slideAttackSpeed) : base(stateMachine)
	{
		_slideAttackSpeed = slideAttackSpeed;
		_player = (Player)stateMachine.Owner;
	}

	internal override void OnEnter(params object[] args)
	{
		// The attack audio is controlled by the animation

		_player.CanChangeFacingDirection = false;
		_attackType = (StateType)args[0];

		StateMachine.CanInteract = false;

		if (SweepRayCastForEnemy())
		{
			_enemy = (Npc)_player.RayCasts["Enemy"].GetCollider();

			if (_enemy.IsVulnerable)
			{
				Engine.TimeScale = 0.1f;
				_player.CanChangeFacingDirection = false;

				_eventsSingleton = StateMachine.GetNode<PlayerEvents>("/root/PlayerEvents");

				_eventsSingleton.EmitGlobalSignal("StateAwaitingForExecutionSpeed");
				_eventsSingleton.ExecutionSpeedSelected += PerformExecution;

				return;
			}
		}

		if (_player.HoldingSword)
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

			if (_attackType == StateType.SlideAttack)
			{
				_player.Velocity = new Vector2(_slideAttackSpeed * (_player.FacingDirection == ActorFacingDirection.Left ? -1 : 1), 0f);
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

		await StateMachine.ToSignal(AnimPlayer, "animation_finished");

		_enemy.Execute(speed);

		StateMachine.ChangeState(typeof(Idle));
	}

	internal override void PhysicsUpdate(double delta)
	{
		if (_attackType != StateType.SlideAttack)
		{
			return;
		}

		_player.MoveAndSlide();
	}

	private void FinishAttack(StringName animName)
	{
		AnimPlayer.AnimationFinished -= FinishAttack;

		StateMachine.ChangeState(typeof(Idle));
	}

	internal override Task OnLeave()
	{
		Engine.TimeScale = 1f;
		StateMachine.CanInteract = true;

		return Task.CompletedTask;
	}

	private bool SweepRayCastForEnemy()
	{
		RayCast2D raycast = _player.RayCasts["Enemy"];

		for (var i = -40; i < -6; i += 2)
		{
			raycast.Position = new Vector2(0f, i);

			raycast.ForceRaycastUpdate();

			if (raycast.IsColliding())
			{
				raycast.Position = new Vector2(0f, -20f);

				return true;
			}
		}

		raycast.Position = new Vector2(0f, -20f);

		return false;
	}
}
