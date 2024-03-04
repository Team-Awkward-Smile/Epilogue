using Epilogue.Global.Enums;
using Epilogue.settings;
using Godot;
using Godot.Collections;
using System.Linq;
using static Godot.DisplayServer;

namespace Epilogue.Global.Singletons;
/// <summary>
///		Class containing every Setting in the game, both the ones that can be edited by the player and the ones who cannot
/// </summary>
public partial class Settings : Node
{
	private const string SETTINGS_FILE = "user://settings.tres";

	/// <summary>
	///		Current game cycle (New Game or New Game+)
	/// </summary>
	public static GameCycle GameCycle { get; private set; } = (GameCycle)ProjectSettings.GetSetting("epilogue/gameplay/game_cycle").AsInt32();

	/// <summary>
	///		Window Mode (Fullscreen, Windowed, etc.) selected by the player
	/// </summary>
	public static WindowMode WindowMode { get; set; }

	/// <summary>
	///		Set of icons to use when playing with a controller
	/// </summary>
	public static InputDeviceBrand ControllerType
	{
		get => s_controllerType;
		set
		{
			s_controllerType = value;

			ProjectSettings.SetSetting("epilogue/controls/controller_type", (int)value);
		}
	}

	/// <summary>
	///		Control scheme (Modern or Retro) selected by the player
	/// </summary>
	public static ControlScheme ControlScheme
	{
		get => s_controlScheme;
		set
		{
			s_controlScheme = value;

			ProjectSettings.SetSetting("epilogue/controls/control_scheme", (int)value);
		}
	}

	private static ControlScheme s_controlScheme = (ControlScheme)ProjectSettings.GetSetting("epilogue/controls/control_scheme").AsInt32();
	private static InputDeviceBrand s_controllerType = (InputDeviceBrand)ProjectSettings.GetSetting("epilogue/controls/controller_type").AsInt32();

	/// <inheritdoc/>
	public override void _EnterTree()
	{
		if (!ResourceLoader.Exists(SETTINGS_FILE))
		{
			GD.Print("Settings file not found. Creating...");

			LoadDefaultSettings();
			SaveSettings();
		}

		GD.Print("Loading settings from file...");

		var settings = ResourceLoader.Load<SettingsResource>(SETTINGS_FILE);

		ControlScheme = settings.ControlScheme;

		foreach (var action in settings.InputMap)
		{
			foreach (var @event in action.Value)
			{
				InputMap.ActionAddEvent(action.Key, @event);
			}
		}

		GD.Print("Loading volumes from file...");

		foreach (var bus in settings.AudioBuses)
		{
			var index = AudioServer.GetBusIndex(bus.Key);

			AudioServer.SetBusVolumeDb(index, bus.Value);
		}

		WindowSetMode(settings.WindowMode);

		WindowMode = settings.WindowMode;
		ControllerType = settings.ControllerType;
	}

	/// <summary>
	///		Saves the current Settings on disk
	/// </summary>
	public static void SaveSettings()
	{
		GD.Print("Saving settings to file...");

		var settings = new SettingsResource()
		{
			ControlScheme = ControlScheme,
			InputMap = GetInputMap(),
			AudioBuses = GetAudioBuses(),
			WindowMode = WindowMode,
			ControllerType = ControllerType
		};

		ResourceSaver.Save(settings, SETTINGS_FILE);
	}

	private static void LoadDefaultSettings()
	{
		GD.Print("Loading default settings...");

		var actions = InputMap.GetActions().Where(a => !a.ToString().StartsWith("ui_") && !a.ToString().EndsWith("modern") && !a.ToString().EndsWith("retro"));

		foreach (var a in actions)
		{
			var events = InputMap.ActionGetEvents($"{a}_{ControlScheme.ToString().ToLower()}");

			foreach (var e in events)
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

		foreach (var action in actions)
		{
			inputMap[action] = InputMap.ActionGetEvents(action);
		}

		return inputMap;
	}

	private static Dictionary<StringName, float> GetAudioBuses()
	{
		var busList = new Dictionary<StringName, float>();

		for (var i = 0; i < AudioServer.BusCount; i++)
		{
			busList.Add(AudioServer.GetBusName(i), AudioServer.GetBusVolumeDb(i));
		}

		return busList;
	}
}
