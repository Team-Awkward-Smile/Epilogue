using Epilogue.Nodes;
using Godot;

namespace Epilogue.Global.Singletons;
/// <summary>
///		Singleton with events emitted by NPCs
/// </summary>
public partial class NpcEvents : Node
{
	/// <summary>
	///		Signal emitted whenever an NPC is executed and a gun is spawned from it
	/// </summary>
	/// <param name="gun">Gun spawned by the NPC</param>
	[Signal] public delegate void GunAcquiredFromNpcEventHandler(Gun gun);
}
