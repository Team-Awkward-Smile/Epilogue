using Epilogue.global.enums;
using Epilogue.settings;
using Godot;
using Godot.Collections;
using System.Linq;

namespace Epilogue.global.singletons;
/// <summary>
///		Class containing every Setting in the game, both the ones that can be edited by the player and the ones who cannot
/// </summary>
public partial class Settings : Node
{
	private const string SETTINGS_FILE = "user://settings.tres";

	/// <summary>
	///		Current game cycle (New Game or New Game+)
	/// </summary>
    public static GameCycle GameCycle { get; private set; } = (GameCycle) ProjectSettings.GetSetting("epilogue/gameplay/game_cycle").AsInt32();

	/// <summary>
	///		Control scheme (Modern or Retro) selected by the player
	/// </summary>
	public static ControlScheme ControlScheme
	{
		get => _controlScheme;
		set
		{
			_controlScheme = value;

			ProjectSettings.SetSetting("epilogue/controls/control_scheme", (int) value);
		}
	}

	private static ControlScheme _controlScheme = (ControlScheme) ProjectSettings.GetSetting("epilogue/controls/control_scheme").AsInt32();

	/// <inheritdoc/>
	public override void _EnterTree()
	{
		if(!ResourceLoader.Exists(SETTINGS_FILE))
		{
			LoadDefaultSettings();
			SaveSettings();
		}

		var settings = ResourceLoader.Load<SettingsResource>(SETTINGS_FILE);

		ControlScheme = settings.ControlScheme;

		foreach(var action in settings.InputMap)
		{
			foreach(var @event in action.Value)
			{
				InputMap.ActionAddEvent(action.Key, @event);
			}
		}
	}

	/// <summary>
	///		Saves the current Settings on disk
	/// </summary>
	public static void SaveSettings()
	{
		var settings = new SettingsResource()
		{
			ControlScheme = ControlScheme,
			InputMap = GetInputMap()
		};

		ResourceSaver.Save(settings, SETTINGS_FILE);
	}

	private static void LoadDefaultSettings()
	{
		var actions = InputMap.GetActions().Where(a => !a.ToString().StartsWith("ui_") && !a.ToString().EndsWith("modern") && !a.ToString().EndsWith("retro"));

		foreach(var a in actions)
		{
			var events = InputMap.ActionGetEvents($"{a}_{ControlScheme.ToString().ToLower()}");

			foreach(var e in events)
			{
				InputMap.ActionAddEvent(a, e);
			}
		}
	}

	private static Dictionary<StringName, Array<InputEvent>> GetInputMap()
	{
		var inputMap = new Dictionary<StringName, Array<InputEvent>>();

		// Gets every "main" action (i.e. excluding every built-in and default action, like "ui_accept" or "move_left_modern")
		var actions = InputMap.GetActions().Where(a => !a.ToString().StartsWith("ui_") && !a.ToString().EndsWith("modern") && !a.ToString().EndsWith("retro"));

		foreach(var action in actions)
		{
			inputMap[action] = InputMap.ActionGetEvents(action);
		}

		return inputMap;
	}
}
