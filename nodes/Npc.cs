using Epilogue.global.enums;

using Godot;

using System.Linq;

namespace Epilogue.nodes;
/// <summary>
///     Base Node for every NPC in the game
/// </summary>
[GlobalClass, Icon("res://nodes/icons/npc.png")]
public abstract partial class Npc : Actor
{
    /// <summary>
    ///     When the NPC's Current HP is equal to or below this value, it will become Vulnerable
    /// </summary>
    [Export] public float VulnerabilityThreshold { get; set; }

    [Export] private float VulnerabilityTimer { get; set; }

    /// <summary>
    ///     Defines if this NPC is Vulnerable
    /// </summary>
    public bool IsVulnerable { get; private set; } = false;

    /// <summary>
    ///     NavigationAgent2D used to control this NPC's pathfinding
    /// </summary>
    private protected NavigationAgent2D NavigationAgent2D { get; set; }

    /// <summary>
    ///     Defines if this NPC can run it's AI logic
    /// </summary>
    public bool CanProcessAI { get; set; } = true;

    /// <summary>
    ///     Resistance of this NPC to the effects of Hestmor's Growling (0 = 0%; 1 = 100%). Negative values increase the effect
    /// </summary>
    public float GrowlEffectResistance { get; set; } = 0f;

    private float _vulnerabilityElapsedTime = 0f;

	/// <inheritdoc/>
	public override void _EnterTree()
	{
        AfterReady += () =>
        {
			NavigationAgent2D = GetChildren().OfType<NavigationAgent2D>().FirstOrDefault();

			if(NavigationAgent2D is null)
			{
				GD.PushWarning($"NavigationAgent2D not set for NPC [{Name}]");
			}
        };
	}

	/// <summary>
	///     Deals damage to this NPC. If it's HP then becomes lower than it's <see cref="VulnerabilityThreshold"/>, it becomes Vulnerable
	/// </summary>
	/// <param name="damage">Ammount of damage to cause</param>
	public override void DealDamage(float damage)
	{
        CurrentHealth -= damage;

        OnDamageTaken(damage, CurrentHealth);

        if(CurrentHealth <= 0 )
        {
            OnHealthDepleted();
        }

        if(!IsVulnerable && CurrentHealth <= VulnerabilityThreshold)
        {
            IsVulnerable = true;
            
            OnVulnerabilityTriggered();
        }
	}

    /// <summary>
    ///     Executes this NPC, running the logic that defines what happens
    /// </summary>
    /// <param name="executionSpeed">Speed of the Execution performed (Slow or Fast)</param>
    public void Execute(ExecutionSpeed executionSpeed)
    {
        CurrentHealth = 0;

        OnExecutionPerformed(executionSpeed);
    }

    /// <summary>
    ///     Heals this NPC. If it was Vulnerable and it's HP goes above it's <see cref="VulnerabilityThreshold"/>, it will return to normal
    /// </summary>
    /// <param name="health">Ammount of HP to heal</param>
	public override void ApplyHealth(float health)
	{
        CurrentHealth += health;

        if(IsVulnerable && (CurrentHealth > VulnerabilityThreshold))
        {
            IsVulnerable = false;
        }
	}

    /// <summary>
    ///     Makes this NPC react to a Growl, taking into account its resistance to the effect
    /// </summary>
    /// <param name="effectStrength">Base Strength of the effect to be applied. This value may be changed depending on the NPC's resistance to the Growl effect</param>
    public void ReactToGrowl(float effectStrength)
    {
        OnGrowl(effectStrength * (1 - GrowlEffectResistance));
    }

	/// <inheritdoc/>
	public override void _PhysicsProcess(double delta)
	{
        if(IsVulnerable)
        {
            _vulnerabilityElapsedTime += (float) delta;

            if(_vulnerabilityElapsedTime >= VulnerabilityTimer)
            {
                _vulnerabilityElapsedTime = 0f;

                OnVulnerabilityExpired();
            }
        }

        if(CanProcessAI)
        {
            ProcessAI(delta);
        }
	}

    /// <summary>
    ///     Method that runs whenever this NPC becomes Vulnerable for the first time
    ///     Default: Change to State "Stun"
    /// </summary>
    private protected virtual void OnVulnerabilityTriggered()
    {
        IsVulnerable = true;
        StateMachine.ChangeState("Stun");
    }

    /// <summary>
    ///     Method that runs whenever this NPC recovers from the Stunned condition
    ///     Default: Change to State "Move"
    /// </summary>
    private protected virtual void OnVulnerabilityExpired()
    {
        StateMachine.ChangeState("Move");
    }

    /// <summary>
    ///     Method that runs whenever this NPC's HP drops to 0
    ///     Default: Change to State "Die"
    /// </summary>
    private protected virtual void OnHealthDepleted()
    {
		StateMachine.ChangeState("Die");
    }

    /// <summary>
    ///     Method that runs whenever this NPC takes damage
    ///     Default: do nothing
    /// </summary>
    /// <param name="damage">Damage taken by the NPC</param>
    /// <param name="currentHp">Current HP of this NPC, after damage was applied</param>
    private protected virtual void OnDamageTaken(float damage, float currentHp) { }

    /// <summary>
    ///     Method that runs whenever this NPC is executed
    ///     Default: Change to State "Executed" on a Slow Execution, or "Die" on a Fast Execution
    /// </summary>
    /// <param name="executionSpeed"></param>
    private protected virtual void OnExecutionPerformed(ExecutionSpeed executionSpeed)
    {
        if(executionSpeed == ExecutionSpeed.Slow)
        {
            StateMachine.ChangeState("Executed");
        }
        else
        {
            StateMachine.ChangeState("Die");
        }
    }

    /// <summary>
    ///     Method that runs whenever this NPC is affected by Hestmor's Growl
    ///     Default: do nothing
    /// </summary>
    /// <param name="effectStrength">Strength of the Growl applied to this NPC</param>
    private protected virtual void OnGrowl(float effectStrength) { }

    /// <summary>
    ///     Method that controls the NPC's AI, running every Physical Frame.
    ///     Must be overriden by each NPC to implement path-finding and other logic
    /// </summary>
    /// <param name="delta">Time, in seconds, since last frame</param>
    private protected abstract void ProcessAI(double delta);
}
