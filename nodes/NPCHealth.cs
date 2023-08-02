using Godot;

namespace Epilogue.nodes;
/// <summary>
///		Node for controlling the HP of enemies. Has all the functionalities of the base Health Node, plus the Execution mechanic
/// </summary>
[GlobalClass]
public partial class NpcHealth : Health
{
	/// <summary>
	///		Event triggered whenever this Actor takes enough damage to become Vulnerable
	/// </summary>
	[Signal] public delegate void ActorIsVulnerableEventHandler();

	/// <summary>
	///		Event triggered if this Actor was Vulnerable long enough to recover
	/// </summary>
	[Signal] public delegate void ActorRecoveredEventHandler();

	[Export] private float ExecutionThreshold { get; set; }

	/// <summary>
	///		Defines if this Actor is vulnerable or not. Different elements can interact with a vulnerable Actor is specific ways
	/// </summary>
	public bool IsVulnerable { get; private set; } = false;

	/// <summary>
	///		Deals the indicated ammount of damage to this Actor. <paramref name="damage"/> must be >= 0
	/// </summary>
	/// <param name="damage">The ammount of damage to deal to this Actor</param>
	public override void DealDamage(float damage)
	{
		base.DealDamage(damage);

		if(CurrentHealth <= ExecutionThreshold)
		{
			IsVulnerable = true;
			EmitSignal(SignalName.ActorIsVulnerable);
		}
	}

	/// <summary>
	///		Heals the indicated ammount of HP to this Actor. <paramref name="health"/> must be >= 0
	/// </summary>
	/// <param name="health">The ammount of HP to recover</param>
	public override void ApplyHealth(float health)
	{
		base.ApplyHealth(health);

		if(CurrentHealth > ExecutionThreshold)
		{
			IsVulnerable = false;
			EmitSignal(SignalName.ActorRecovered);
		}
	}
}

