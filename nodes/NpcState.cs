using Epilogue.global.singletons;

using Godot;
using System.Linq;

namespace Epilogue.nodes;
/// <summary>
///		Base class for States belonging to NPCs
/// </summary>
public partial class NpcState : State
{
	/// <summary>
	///		Reference to the NPC who owns this State
	/// </summary>
	public Npc Npc { get; private set; }

	/// <summary>
	///		Reference to the Player character
	/// </summary>
    public Player Player { get; set; }

    private protected override void AfterReady()
	{
		Npc = Owner as Npc;
		Player = GetTree().GetLevel().Player;
	}
}
