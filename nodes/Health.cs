using Godot;
using System.Collections.Generic;

namespace Epilogue.nodes;
/// <summary>
///		Node responsible for everything related to the Health of an Actor
/// </summary>
[GlobalClass, Icon("res://nodes/health.png"), Tool]
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

	public override string[] _GetConfigurationWarnings()
	{
		var warnings = new List<string>();

		if(MaxHealth == 0)
		{
			warnings.Add("The Max Health of this Actor is set to 0");
		}

		return warnings.ToArray();
	}

	public override void _Ready()
	{
		// TODO: this is just a draft of the Node. It needs way more methods and ways to set it's values at runtime
		CurrentHealth = MaxHealth;
	}
}
