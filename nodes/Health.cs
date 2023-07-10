using Godot;

namespace Epilogue.nodes;
/// <summary>
///		Node responsible for everything related to the Health of an Actor
/// </summary>
[GlobalClass]
public partial class Health : Node
{
	/// <summary>
	///		Max Health of the Actor
	/// </summary>
	[Export] public int MaxHealth { get; private set; }

	/// <summary>
	///		Current Health of the Actor
	/// </summary>
	public int CurrentHealth { get; private set; }

	public override void _Ready()
	{
		// TODO: this is just a draft of the Node. It needs way more methods and ways to set it's values at runtime
		CurrentHealth = MaxHealth;
	}
}
