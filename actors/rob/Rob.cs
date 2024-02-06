using Epilogue.Actors.rob.states;
using Epilogue.Global.Enums;
using Epilogue.Nodes;
using Godot;
using Godot.Collections;

namespace Epilogue.Actors.rob;
/// <summary>
/// 	Base class for the NPC Rob, with the logic necessary for it's AI
/// </summary>
public partial class Rob : Npc
{
	/// <summary>
	/// 	The time (in seconds) it takes for Rob to be able to shoot again
	/// </summary>
	public float ShotCooldown { get; set; }

	/// <summary>
	/// 	The time (in seconds) it takes for Rob to be able to perform another melee attack
	/// </summary>
	public float MeleeCooldown { get; set; }

	/// <summary>
	/// 	Time (in seconds) since Rob last performed an attack
	/// </summary>
	public float AttackTimer { get; set; } = 0f;

	/// <summary>
	/// 	Duration (in seconds) that Rob will remain fleeing from Hestmor
	/// </summary>
	public float FleeDuration { get; set; } = 0f;

	/// <summary>
	/// 	Defines if Rob can shoot a projectile at Hestmor or not
	/// </summary>
	public bool CanShoot { get; set; } = true;

	/// <summary>
	/// 	Speed multiplier used by Rob to increase/decrease it's movement speed, regardless of it's actual value
	/// </summary>
	public float SpeedMultiplier { get; set; } = 1f;

	private protected override bool UseDefaultPathfinding => true;

	/// <inheritdoc/>
	public override Dictionary<DamageType, float> DamageModifiers { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

	private protected override void ProcessFrame(double delta)
	{
		AttackTimer += (float)delta;
	}

	private protected override void OnDamageTaken(float damage, float currentHp, DamageType damageType)
	{
		CanShoot = false;
	}

	private protected override void OnGrowl(GrowlType growlType)
	{
		FleeDuration = growlType switch
		{
			GrowlType.Weak => 2f,
			GrowlType.Medium => 5f,
			_ => 10f
		};
	}

	private protected override void OnVulnerabilityTriggered()
	{
		_npcStateMachine.ChangeState(typeof(Stun));
	}

    private protected override void OnHealthDepleted(DamageType damageType)
    {
        _npcStateMachine.ChangeState(typeof(Die));
    }

	private protected override void OnExecutionPerformed(ExecutionSpeed executionSpeed)
	{
		_npcStateMachine.ChangeState(executionSpeed == ExecutionSpeed.Slow ? typeof(Executed) : typeof(Die));
	}

	private protected override void OnPlayerDeath()
	{
		_npcStateMachine.ChangeState(typeof(Wander));
	}
}