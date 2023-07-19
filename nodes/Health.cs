using Epilogue.global.enums;
using Godot;

namespace Epilogue.nodes;
/// <summary>
///		Node for controlling an Actor's HP
/// </summary>
[GlobalClass]
public partial class Health : Node
{
	// TODO: 68 - Implement an Editor Plugin to show/hide the Glory Kill variables when this property is set
	[Export] private ActorType ActorType { get; set; }
	[Export] private float MaxHealth { get; set; }
	[Export] private float GloryKillThreshold { get; set; }

	/// <summary>
	///		Current HP of this Actor. On NPCs, setting this value to a number equal to or lower than GloryKillThreshold will automatically set IsVulnerable to true
	/// </summary>
	public float CurrentHealth 
	{ 
		get => _currentHealth; 
		private set
		{
			_currentHealth = value;

			if(ActorType != ActorType.Player)
			{
				IsVulnerable = _currentHealth <= GloryKillThreshold;
			}
		}
	}

	/// <summary>
	///		Defines if this Actor is vulnerable or not. Different elements can interact with a vulnerable Actor is specific ways
	/// </summary>
	public bool IsVulnerable { get; private set; } = false;

	private Actor _actor;
	private float _currentHealth;

    public override void _Ready()
	{
		_actor = (Actor) Owner;

		CurrentHealth = MaxHealth;
	}

	public virtual void DealDamage(float damage)
	{
		if(damage < 0)
		{
			GD.PushWarning($"Trying to deal negative damage to Actor [{_actor.Name}]. Use ApplyHealth instead");
			return;
		}

		CurrentHealth -= damage;

		if(CurrentHealth <= 0)
		{
			_actor.QueueFree();
		}

		if(IsVulnerable)
		{
			_actor.StateMachine.ChangeState("Vulnerable");
		}
	}

	public virtual void ApplyHealth(float health) 
	{
		if(health < 0)
		{
			GD.PushWarning($"Trying to apply negative health to Actor [{_actor.Name}]. Use DealDamage instead");
			return;
		}

		CurrentHealth += health;
	}
}
