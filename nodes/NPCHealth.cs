using Godot;

namespace Epilogue.nodes;
/// <summary>
///		Node for controlling the HP of enemies. Has all the functionalities of the base Health Node, plus the Execution mechanic
/// </summary>
[GlobalClass]
public partial class NPCHealth : Health
{
	[Signal] public delegate void ActorIsVulnerableEventHandler();
	[Signal] public delegate void ActorRecoveredEventHandler();

	[Export] private float ExecutionThreshold { get; set; }

	/// <summary>
	///		Defines if this Actor is vulnerable or not. Different elements can interact with a vulnerable Actor is specific ways
	/// </summary>
	public bool IsVulnerable { get; private set; } = false;

	public override void DealDamage(float damage)
	{
		base.DealDamage(damage);

		if(CurrentHealth <= ExecutionThreshold)
		{
			IsVulnerable = true;
			EmitSignal(SignalName.ActorIsVulnerable);
		}
	}

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

