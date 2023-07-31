using Epilogue.util;
using Godot;

namespace Epilogue.actors.hestmor;
/// <summary>
///		Node used to control if the player pressed the Select button in Retro Mode to toggle between Walking and Running
/// </summary>
public partial class MovementInputManager : Node
{
	private bool _retroModeEnabled;

	/// <summary>
	///		Defines if the player toggled the Run mode while playing in Retro Mode
	/// </summary>
	public bool RunEnabled { get; set; } = false;

	public override void _Ready()
	{
		// TODO: 68 - Reset this value when the Input Mode is changed during gameplay
		_retroModeEnabled = !ProjectSettings.GetSetting("global/use_modern_controls").AsBool();
	}

	public override void _UnhandledInput(InputEvent @event)
	{
		if(_retroModeEnabled && Input.IsActionJustPressed("toggle_walk_run"))
		{
			RunEnabled = !RunEnabled;
		}
		else if(@event.IsAction(InputUtils.GetInputActionName("run_modifier")))
		{
			RunEnabled = Input.IsActionPressed(InputUtils.GetInputActionName("run_modifier"));
		}
	}
}
