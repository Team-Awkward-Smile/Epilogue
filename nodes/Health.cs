using Godot;

namespace Epilogue.nodes;
[GlobalClass]
public partial class Health : Node
{
	[Export] public int MaxHealth { get; private set; }
	[Export] public bool IsInGloryKillMode { get; private set; }
	public int CurrentHealth { get; private set; }

	public override void _Ready()
	{
		CurrentHealth = MaxHealth;
	}

	public void ApplyDamage(int damage)
	{
		CurrentHealth -= damage;

		if(CurrentHealth <= 0)
		{
			IsInGloryKillMode = true;
			((Actor) Owner).Sprite.Modulate = new Color(1f, 0f, 0f);
		}
	}

	public void GloryKill()
	{
		Owner.QueueFree();
	}
}
