using Epilogue.global.enums;
using Godot;

namespace Epilogue.global.singletons;
/// <summary>
///		Class containing every Setting in the game, both the ones that can be edited by the player and the ones who cannot
/// </summary>
public partial class Settings : Node
{
	/// <summary>
	///		Control scheme (Modern or Retro) selected by the player
	/// </summary>
	public static ControlSchemeEnum ControlScheme { get; private set; } = ProjectSettings.GetSetting("global/use_modern_controls").AsBool() ? ControlSchemeEnum.Modern : ControlSchemeEnum.Retro;

	/// <summary>
	///		Current game cycle (New Game or New Game+)
	/// </summary>
    public static GameCycle GameCycle { get; private set; } = (GameCycle) ProjectSettings.GetSetting("global/game_cycle").AsInt32();

    /// <inheritdoc/>
    public override void _Ready()
	{
		// TODO: this should be set from the Settings screen
		GD.Print($"Control Scheme: {ControlScheme}");
	}
}
