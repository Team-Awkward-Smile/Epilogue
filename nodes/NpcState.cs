using Godot;
using System.Linq;

namespace Epilogue.nodes;
/// <summary>
///		Base class for States belonging to NPCs
/// </summary>
public partial class NpcState : State
{
	/// <summary>
	///		Reference to the NavigationAgent2D used by this NPC to move around
	/// </summary>
	protected NavigationAgent2D NavigationAgent { get; private set; }

	/// <summary>
	///		Reference to the NPC who owns this State
	/// </summary>
	public Npc Npc { get; private set; }

	private protected override void AfterReady()
	{
		NavigationAgent = Owner.GetChildren().OfType<NavigationAgent2D>().FirstOrDefault();
		Npc = Owner as Npc;
	}
}
