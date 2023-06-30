using Epilogue.global.enums;
using Godot;

namespace Epilogue.global.singletons;
public partial class Settings : Node
{
	public static ControlScheme ControlScheme { get; private set; } = ProjectSettings.GetSetting("global/use_modern_controls").AsBool() ? ControlScheme.Modern : ControlScheme.Retro;

	public override void _Ready()
	{
		GD.Print($"Control Scheme: {ControlScheme}");
	}
}
