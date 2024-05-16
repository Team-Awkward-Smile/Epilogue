using Epilogue.Actors.MossPlant.States;
using Epilogue.Const;
using Epilogue.Global.Enums;
using Epilogue.Nodes;
using Godot;
using Godot.Collections;

namespace Epilogue.Actors.MossPlant;
/// <summary>
///		Class that manages the AI of the Moss Plant of Guwama
/// </summary>
public partial class MossPlant : Npc
{
	/// <summary>
	///		Defines if the player is within range
	/// </summary>
	public bool IsPlayerInRange { get; private set; }

	/// <summary>
	///		Time (in seconds) since the last attack
	/// </summary>
	public float AttackTimer { get; set; }

	/// <summary>
	///		Defines if there's a projectile active
	/// </summary>
	public bool IsProjectileActive { get; set; } = false;

	private double _projectileRaycastTimer;
	private RayCast2D _projectileRaycast;
	private Node2D _projectileSpawnPoint;

	/// <inheritdoc/>
	public override Dictionary<DamageType, float> DamageModifiers { get; set; } = new()
	{
		{ DamageType.Unarmed, +10 }
	};

	private protected override bool UseDefaultPathfinding => false;

	/// <inheritdoc/>
	public override void _Ready()
	{
		base._Ready();

		_projectileRaycast = RayCasts["Projectile"];
		_projectileSpawnPoint = GetNode<Node2D>("ProjectileSpawnPoint");
	}

	/// <summary>
	///		Method that runs when a projectile that was reflected by the player reaches a Moss Plant
	/// </summary>
	public void OnProjectileReturned()
	{
		_npcStateMachine.ChangeState(typeof(Die));
	}

	private protected override void OnDamageTaken(float damage, float currentHp, DamageType damageType)
	{
		return;
	}

	private protected override void OnExecutionPerformed(ExecutionSpeed executionSpeed)
	{
		return;
	}

	private protected override void OnGrowl(GrowlType growlType)
	{
		return;
	}

	private protected override void OnHealthDepleted(DamageType damageType)
	{
		_npcStateMachine.ChangeState(typeof(Die));
	}

	private protected override void OnPlayerDeath()
	{
		_npcStateMachine.ChangeState(typeof(Idle));
	}

	private protected override void OnVulnerabilityTriggered()
	{
		return;
	}

	private protected override void ProcessFrame(double delta)
	{
		AttackTimer += (float)delta;

		_projectileRaycastTimer += delta;

		if (_projectileRaycastTimer >= 0.1f)
		{
			_projectileRaycastTimer = 0f;

			_projectileRaycast.TargetPosition = (Player.GlobalPosition - new Vector2(0f, Constants.PLAYER_HEIGHT / 2) - GlobalPosition).Normalized() * 200f;
			_projectileRaycast.ForceRaycastUpdate();

			IsPlayerInRange = _projectileRaycast.IsColliding() && _projectileRaycast.GetCollider() is Player;

			if (IsPlayerInRange && !IsProjectileActive)
			{
				_projectileSpawnPoint.Rotation = Mathf.Atan2(_projectileRaycast.TargetPosition.Y, _projectileRaycast.TargetPosition.X);
			}
			else
			{
				AttackTimer = 0f;
			}
		}
	}
}
