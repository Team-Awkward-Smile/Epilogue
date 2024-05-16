using Epilogue.Actors.Icarasia.Enums;
using Epilogue.Actors.Icarasia.States;
using Epilogue.Global.Enums;
using Epilogue.Nodes;
using Godot;
using Godot.Collections;

namespace Epilogue.Actors.Icarasia;
/// <summary>
///		Base class for the Icarasia NPC with AI
/// </summary>
public partial class Icarasia : Npc
{
	/// <summary>
	///		Preferred way this Icarasia will try to attack the player
	/// </summary>
	[Export] public PreferredAttack PreferredAttack { get; set; }

	/// <summary>
	///		Sets if this Icarasia will wander in search for Terra Bischem, or stay still
	/// </summary>
	[Export] public bool NearTerraBischem { get; private set; } = false;

	[Export] private float _detectionDistance = 200f;

	private protected override bool UseDefaultPathfinding => false;

	/// <inheritdoc/>
	public override Dictionary<DamageType, float> DamageModifiers { get; set; } = new()
	{
		{ DamageType.Unarmed, +5 },
		{ DamageType.GunThrow, +10 },
		{ DamageType.Dung, -4 }
	};

	/// <summary>
	///		Cooldown (in seconds) between shots
	/// </summary>
	public float ShotCooldown { get; set; }

	/// <summary>
	///		Cooldown (in seconds) between stings
	/// </summary>
	public float StingerCooldown { get; set; }

	/// <summary>
	///		Time (in seconds) since the last attack
	/// </summary>
	public float AttackTimer { get; set; }

	/// <summary>
	///		Defines if the player is detected or not
	/// </summary>
	public bool IsPlayerDetected { get; set; }

	/// <summary>
	///		Angle of the shot to be used for the attack. Null if the player cannot be hit.
	///		Updated every 0.1 second
	/// </summary>
	public int? ShotAngle { get; set; }

	/// <summary>
	///		Direction to be used for the sting attack. Null if the player cannot be hit.
	///		Updated every 0.1 second
	/// </summary>
	public StingDirection? StingDirection { get; set; }

	/// <summary>
	///		Distance (in units) between the Icarasia and the player.
	///		Updated every frame
	/// </summary>
	public float DistanceToPlayer { get; private set; }

	private int[] _projectileAngles = new[] { 0, 45, 135, 180, 225, 315 };
	private float _projectileSweepTimer;
	private float _stingerSweepTimer;
	private bool _isIsCombatMode;

	/// <summary>
	///     Checks detection of player
	///     When player is detected, sweeps an area around itself every 0.1 second to see if a shot is aligned
	/// </summary>
	private protected override void ProcessFrame(double delta)
	{
		AttackTimer += (float)delta;

		DistanceToPlayer = Player.GlobalPosition.DistanceTo(GlobalPosition);

		if (!IsPlayerDetected && DistanceToPlayer <= _detectionDistance)
		{
			IsPlayerDetected = true;
		}

		_projectileSweepTimer += (float)delta;
		_stingerSweepTimer += (float)delta;

		if (IsPlayerDetected && _projectileSweepTimer >= 0.1f)
		{
			_projectileSweepTimer = 0f;

			ShotAngle = SweepProjectileRayCastForPlayer();
		}

		if (IsPlayerDetected && _stingerSweepTimer >= 0.1f)
		{
			_stingerSweepTimer = 0f;

			StingDirection = SweepStingerRaycastForPlayer();
		}
	}

	private StingDirection? SweepStingerRaycastForPlayer()
	{
		var directions = new Vector2[] { new(30, 0), new(-30, 0), new(0, 30) };
		var raycast = RayCasts["Stinger"];

		raycast.Enabled = true;

		foreach (Vector2 direction in directions)
		{
			raycast.TargetPosition = direction;
			raycast.ForceRaycastUpdate();

			if (raycast.IsColliding() && raycast.GetCollider() is Player)
			{
				raycast.Enabled = false;

				return (direction.X != 0) ? Enums.StingDirection.Forward : Enums.StingDirection.Down;
			}
		}

		raycast.Enabled = false;

		return null;
	}

	private int? SweepProjectileRayCastForPlayer()
	{
		var raycast = RayCasts["Projectile"];

		raycast.Enabled = true;

		foreach (int angle in _projectileAngles)
		{
			raycast.RotationDegrees = angle;
			raycast.ForceRaycastUpdate();

			if (raycast.IsColliding() && (raycast.GetCollider() is Player))
			{
				raycast.Enabled = false;

				return angle;
			}
		}

		raycast.Enabled = false;

		return null;
	}

	private protected override void OnVulnerabilityTriggered()
	{
		_npcStateMachine.ChangeState(typeof(Vulnerable));
	}

	private protected override void OnHealthDepleted(DamageType damageType)
	{
		if (damageType == DamageType.Unarmed)
		{
			// The Icarasia will die after 3 seconds if no Execution is performed
			_npcStateMachine.ChangeState(typeof(Vulnerable), 3f);
		}
		else
		{
			_npcStateMachine.ChangeState(typeof(Die));
		}
	}

	private protected override void OnDamageTaken(float damage, float currentHp, DamageType damageType)
	{
		var blinkTime = damageType == DamageType.Unarmed ? 0.4f : 0.2f;

		if (CurrentGrowlInEffect is GrowlType.Strong or GrowlType.Weak)
		{
			_npcStateMachine.ChangeState(typeof(Stun), 2f);
		}
		else
		{
			_npcStateMachine.ChangeState(typeof(Push), blinkTime);

			if (CurrentGrowlInEffect is GrowlType.Medium)
			{
				_ = _npcStateMachine.Connect(StateMachine.SignalName.StateExited,
					Callable.From(() => _npcStateMachine.ChangeState(typeof(Flee), 100f, 2f)),
					(uint)ConnectFlags.OneShot);
			}
		}
	}

	private protected override void OnExecutionPerformed(ExecutionSpeed executionSpeed)
	{
		_npcStateMachine.ChangeState(typeof(Die), executionSpeed);
	}

	private protected override void OnGrowl(GrowlType growlType)
	{
		switch (growlType)
		{
			case GrowlType.Weak:
				_npcStateMachine.ChangeState(typeof(Stun), 1.5f);

				DamageModifiers[DamageType.Unarmed] = 1.5f;

				// Returns the Melee weakness to normal after leaving the Stun State
				_ = _npcStateMachine.Connect(StateMachine.SignalName.StateExited,
						Callable.From(() => DamageModifiers[DamageType.Unarmed] = 1f),
						(uint)ConnectFlags.OneShot);

				break;

			case GrowlType.Medium:
				_npcStateMachine.ChangeState(typeof(Flee), 50f, 2f);

				DamageModifiers[DamageType.Unarmed] = 1.8f;

				// Returns the Melee weakness to normal after leaving the Stun State
				_ = _npcStateMachine.Connect(StateMachine.SignalName.StateExited,
						Callable.From(() => DamageModifiers[DamageType.Unarmed] = 1f),
						(uint)ConnectFlags.OneShot);

				break;

			case GrowlType.Strong:
				_npcStateMachine.ChangeState(typeof(Flee), 100f, 3f);

				DamageModifiers[DamageType.Unarmed] = 2.5f;

				// Returns the Melee weakness to normal after leaving the Stun State
				_ = _npcStateMachine.Connect(StateMachine.SignalName.StateExited,
						Callable.From(() => DamageModifiers[DamageType.Unarmed] = 1f),
						(uint)ConnectFlags.OneShot);

				break;
		}
	}

	private protected override void OnPlayerDeath()
	{
		_npcStateMachine.ChangeState(typeof(Wander));
	}

	private protected override void OnProjectileNotification()
	{
		return;
	}

	private protected override void OnDesperationTriggered()
	{
		throw new System.NotImplementedException();
	}
}
