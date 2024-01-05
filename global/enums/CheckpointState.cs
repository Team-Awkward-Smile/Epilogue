namespace Epilogue.Global.Enums;
/// <summary>
///		State of a checkpoint
/// </summary>
public enum CheckpointState
{
	/// <summary>
	///		This checkpoint was not triggered by the player
	/// </summary>
	Inactive,

	/// <summary>
	///		This is the most recent checkpoint
	/// </summary>
	Current,

	/// <summary>
	///		This checkpoint was triggered and then replaced by a new one
	/// </summary>
	Used
}
