using Epilogue.global.enums;

using Godot;
using Godot.Collections;

namespace Epilogue.settings;
/// <summary>
///		Resource to be used when reading and writing Settings to disk, allowing for a faster and easier manipulation
/// </summary>
public partial class SettingsResource : Resource
{
	/// <summary>
	///		Control Scheme selected by the player
	/// </summary>
	[Export] public ControlSchemeEnum ControlScheme { get; set; }

	/// <summary>
	///		Controls used during the game. Can have the default value for the corresponsing Control Scheme, or can be edited by the player to suit their liking
	/// </summary>
	[Export] public Dictionary<StringName, Array<InputEvent>> InputMap { get; set; } = new();
}
