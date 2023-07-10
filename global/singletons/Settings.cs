using Epilogue.global.enums;
using Godot;

namespace Epilogue.global.singletons;
public partial class Settings : Node
{
	public static ControlSchemeEnum ControlScheme { get; private set; } = ProjectSettings.GetSetting("global/use_modern_controls").AsBool() ? ControlSchemeEnum.Modern : ControlSchemeEnum.Retro;

	public override void _Ready()
	{
		GD.Print($"Control Scheme: {ControlScheme}");
	}
}
