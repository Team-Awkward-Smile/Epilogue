using Godot;

namespace Epilogue.nodes;
[GlobalClass]
public partial class Health : Node
{
	[Export] public int MaxHealth { get; private set; }
	public int CurrentHealth { get; private set; }

	public override void _Ready()
	{
		CurrentHealth = MaxHealth;
	}
}
