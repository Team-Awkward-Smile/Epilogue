using Epilogue.global.enums;

using Godot;
using Godot.Collections;

namespace Epilogue.settings;
public partial class SettingsResource : Resource
{
	[Export] public ControlSchemeEnum ControlScheme { get; set; }
	[Export] public Dictionary<StringName, Array<InputEvent>> InputMap { get; set; } = new();
}
