using Godot;

using System.Collections.Generic;

namespace Epilogue.global.singletons;
/// <summary>
///		Singleton responsible for storing data about objects in a level so they can be properly reloaded once the level starts
/// </summary>
public partial class CheckpointManager : Node
{
	/// <summary>
	///     ID of the current Checkpoint of the Level (i.e. the last one triggered by the player)
	/// </summary>
	public int? CurrentCheckpointID { get; set; }

	/// <summary>
	///     List of IDs of Checkpoints that were already triggered and cannot be used again
	/// </summary>
	public List<int> UsedCheckpointsIDs { get; set; } = new();

	/// <summary>
	///		Resets every property from the singleton, making it ready to be used in a new Level
	/// </summary>
	public void Reset()
	{
		CurrentCheckpointID = null;
		UsedCheckpointsIDs = new();
	}
}
