using Godot;

namespace Epilogue.nodes;
/// <summary>
///     Base Node for every NPC in the game
/// </summary>
[GlobalClass, Icon("res://nodes/icons/npc.png")]
public partial class Npc : Actor
{
    /// <summary>
    ///     When the NPC's Current HP is equal to below this value, it will become Vulnerable
    /// </summary>
    [Export] public float VulnerabilityThreshold { get; set; }

    /// <summary>
    ///     Defines if this NPC is Vulnerable
    /// </summary>
    public bool IsVulnerable { get; private set; }

    /// <summary>
    ///     Deals damage to this NPC. If it's HP then becomes lower than it's <see cref="VulnerabilityThreshold"/>, it becomes Vulnerable
    /// </summary>
    /// <param name="damage">Ammount of damage to cause</param>
	public override void DealDamage(float damage)
	{
        CurrentHealth -= damage;

        if(CurrentHealth <= 0 )
        {
            QueueFree();
        }

        if(CurrentHealth <= VulnerabilityThreshold)
        {
            IsVulnerable = true;
        }
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
}
