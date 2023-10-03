using Epilogue.global.enums;

using Godot;
using Godot.Collections;
using static Godot.DisplayServer;

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
	///		Controls used during the game. Can have the default value for the corresponding Control Scheme, or can be edited by the player to suit their liking
	/// </summary>
	[Export] public Dictionary<StringName, Array<InputEvent>> InputMap { get; set; } = new();

	/// <summary>
	///		List of each audio bus with the volume (in dB scale) set by the player
	/// </summary>
	[Export] public Dictionary<StringName, float> AudioBuses { get; set; } = new();

	/// <summary>
	///		Window Mode (Fullscreen, Windowed, etc.) selected by the player
	/// </summary>
	[Export] public WindowMode WindowMode { get; set; }

	/// <summary>
	///		Set of icons to use when playing with a controller
	/// </summary>
    [Export] public InputDeviceBrand ControllerType { get; set; }
}
