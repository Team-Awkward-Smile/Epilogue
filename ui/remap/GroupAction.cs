using System.Collections.Generic;

namespace Epilogue.UI.remap;
/// <summary>
///		Object representing a group of actions that can be remapped at once
/// </summary>
public class GroupAction
{
	/// <summary>
	///		Label of this group that is displayed to the player (eg "Jump / Vault / Climb Ledge")
	/// </summary>
	public string Label { get; set; }

	/// <summary>
	///		List of every action that forms this group (eg "jump", "vault", "climb_ledge")
	/// </summary>
	public List<string> Actions { get; set; }
}
