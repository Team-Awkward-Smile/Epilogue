using Epilogue.global.enums;
using Godot;

namespace Epilogue.global.singletons;
public partial class Settings : Node
{
	/// <summary>
	///		Control scheme (Modern or Retro) selected by the player
	/// </summary>
	public static ControlSchemeEnum ControlScheme { get; private set; } = ProjectSettings.GetSetting("global/use_modern_controls").AsBool() ? ControlSchemeEnum.Modern : ControlSchemeEnum.Retro;

	public override void _Ready()
	{
		// TODO: this should be set from the Settings screen
		GD.Print($"Control Scheme: {ControlScheme}");
	}
}
