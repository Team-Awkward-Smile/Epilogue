using Godot;

namespace Epilogue.nodes;
/// <summary>
///     Base Node for every NPC in the game
/// </summary>
[GlobalClass, Icon("res://nodes/icons/npc.png")]
public partial class Npc : Actor
{
    /// <summary>
    ///     When the NPC's Current HP is equal to or below this value, it will become Vulnerable
    /// </summary>
    [Export] public float VulnerabilityThreshold { get; set; }

    [Export] private float VulnerabilityTimer { get; set; }

    /// <summary>
    ///     Defines if this NPC is Vulnerable
    /// </summary>
    public bool IsVulnerable { get; private set; }

    private float _timer = 0f;

    /// <summary>
    ///     Deals damage to this NPC. If it's HP then becomes lower than it's <see cref="VulnerabilityThreshold"/>, it becomes Vulnerable
    /// </summary>
    /// <param name="damage">Ammount of damage to cause</param>
	public override void DealDamage(float damage)
	{
        CurrentHealth -= damage;

        if(CurrentHealth <= 0 )
        {
            OnHealthDepleted();
        }

        if(CurrentHealth <= VulnerabilityThreshold)
        {
            IsVulnerable = true;
            
            OnVulnerabilityTriggered();
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

	public override void _PhysicsProcess(double delta)
	{
        if(IsVulnerable)
        {
            _timer += (float) delta;

            if(_timer >= VulnerabilityTimer)
            {
                _timer = 0f;

                OnVulnerabilityExpired();
            }
        }
	}

	private protected virtual void OnVulnerabilityTriggered() { }

    private protected virtual void OnVulnerabilityExpired() { }

    private protected virtual void OnHealthDepleted() { }
}
