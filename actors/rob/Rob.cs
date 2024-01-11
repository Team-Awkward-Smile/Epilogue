using Epilogue.actors.rob.states;
using Epilogue.Global.Enums;
using Epilogue.Nodes;
using Godot;

namespace Epilogue.actors.rob;
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

	private protected override void ProcessFrame(double delta)
	{
		AttackTimer += (float) delta;
	}

	private protected override void OnDamageTaken(float damage, float currentHp)
	{
		CanShoot = false;
	}

	private protected override void OnGrowl(float effectStrength)
	{
		FleeDuration = effectStrength / 10f;
	}

	private protected override void OnStunExpired()
	{
		SpeedMultiplier = 1.2f;

		GetNode<HitBox>("FlipRoot/HitBox").BonusDamage = 2f;
	}

    private protected override void OnVulnerabilityTriggered()
    {
        _npcStateMachine.ChangeState(typeof(Stun));
    }

    private protected override void OnHealthDepleted()
    {
        _npcStateMachine.ChangeState(typeof(Die));
    }

    private protected override void OnExecutionPerformed(ExecutionSpeed executionSpeed)
    {
        _npcStateMachine.ChangeState(executionSpeed == ExecutionSpeed.Slow ? typeof(Executed) : typeof(Die));
    }

    private protected override void OnStunTriggered()
    {
        _npcStateMachine.ChangeState(typeof(Stun));
    }

    private protected override void OnPlayerDeath()
    {
        _npcStateMachine.ChangeState(typeof(Wander));
    }
}