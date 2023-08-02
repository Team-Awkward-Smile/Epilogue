using Godot;

namespace Epilogue.nodes;
/// <summary>
///     Base Node for every NPC in the game
/// </summary>
[GlobalClass, Icon("res://nodes/icons/npc.png")]
public partial class Npc : Actor
{
	/// <summary>
	///     Health Node belonging to this NPC
	/// </summary>
	public NpcHealth Health { get; private set; }

	private protected override void AfterReady()
	{
		Health = GetNode<NpcHealth>("NpcHealth");
	}
}
