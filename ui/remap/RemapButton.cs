using Epilogue.Global.Enums;
using Epilogue.Global.Singletons;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Epilogue.UI.Remap;
/// <summary>
///		Button used in the Remap Controls Screen to display the correct icons and allow the player to remap the controls
/// </summary>
public partial class RemapButton : Button
{
	/// <summary>
	///		Defines if this button can read any kind of input (Modern Control Scheme), or only digital inputs (Retro Control Scheme)
	/// </summary>
	public bool CanReadModernInputs { get; set; } = true;

	/// <summary>
	///		Type of input read by this button
	/// </summary>
	public InputDeviceType InputType { get; set; }

	/// <summary>
	///		Action assigned to this button that will be remapped as one if the player assigns a new event to them
	/// </summary>
	public List<string> Actions { get; set; } = new();

	/// <summary>
	///		Type of button, used to correctly display the icons to the player
	/// </summary>
	public RemapButtonType ButtonType { get; set; }

	/// <summary>
	///		Event triggered whenever this button is clicked and is ready to remap an action
	/// </summary>
	/// <param name="inputType"></param>
	[Signal] public delegate void ActionWasSelectedEventHandler(InputDeviceType inputType);

	/// <summary>
	///		InputEvent assigned to this button. Setting a new value will automatically update it's icon
	/// </summary>
	public InputEvent Event
	{
		get => _event;
		set
		{
			_event = value;

			RemoveThemeColorOverride("font_color");
			RemoveThemeFontSizeOverride("font_size");

			Text = null;
			Icon = null;

			// No Event (this button is empty)
			if(_event is null)
			{
				Text = "Unbound";

				AddThemeColorOverride("font_color", new Color(0.47f, 0.47f, 0.47f));
			}
			else if(_event is InputEventKey key)
			{
				// Key from the keyboard
				var unicodeChar = ((char) key.Unicode).ToString();

				if(unicodeChar != "\0" && unicodeChar != " ")
				{
					Text = unicodeChar.ToUpper();
				}
				else
				{
					Text = key.PhysicalKeycode == Key.Escape ? "Esc" : key.PhysicalKeycode.ToString();
				}

				AddThemeFontSizeOverride("font_size", 40);
			}
			else
			{
				if(_event is InputEventMouse)
				{
					// Mouse button
					Text = _event.AsText() + "\n[Icon Pending]";
				}
				else
				{
					// Controller key
					Icon = InputDeviceManager.GetKeyIcon(_event);
				}
			}
		}
	}

	private readonly List<Type> _validInputTypes = new();
	private InputEvent _event = null;

	/// <inheritdoc/>
	public override void _Ready()
	{
		SetValidInputs();
		ButtonDown += StartWaitingForInput;

		CustomMinimumSize = new Vector2(64f, 64f);
		ExpandIcon = true;
		IconAlignment = HorizontalAlignment.Center;
	}

	/// <summary>
	///		Sets a new list of valid InputEvents that can be detected by this button.
	///		While remapping, any invalid input will be ignored automatically
	/// </summary>
	private void SetValidInputs()
	{
		_validInputTypes.Clear();

		CanReadModernInputs = Settings.ControlScheme == ControlScheme.Modern;

		if(InputType == InputDeviceType.PC)
		{
			_validInputTypes.Add(typeof(InputEventKey));

			if(CanReadModernInputs)
			{
				_validInputTypes.Add(typeof(InputEventMouseButton));
			}
		}
		else
		{
			_validInputTypes.Add(typeof(InputEventJoypadButton));

			if(CanReadModernInputs)
			{
				_validInputTypes.Add(typeof(InputEventJoypadMotion));
			}
		}
	}

	/// <summary>
	///		Updates the text and/or icon of this button.
	///		Used when the update needs to happen without the Event property being updated (when resetting to default, for instance)
	/// </summary>
	private void UpdateIconAndText()
	{
		var actionName = Actions.First();
		var events = InputMap.ActionGetEvents(actionName);

		switch(ButtonType)
		{
			case RemapButtonType.PcPrimary:
				Event = events.Where(e => e is InputEventKey or InputEventMouse).FirstOrDefault();
				break;

			case RemapButtonType.PcSecondary:
				Event = events.Where(e => e is InputEventKey or InputEventMouse).Skip(1).FirstOrDefault();
				break;

			case RemapButtonType.Controller:
				Event = events.Where(e => e is InputEventJoypadButton or InputEventJoypadMotion).FirstOrDefault();
				break;
		}
	}

	/// <summary>
	///		Updates the event of the Actions assigned to this button to the one read from the player
	/// </summary>
	/// <param name="event"></param>
	public void UpdateMapping(InputEvent @event)
	{
		foreach(var action in Actions)
		{
			if(Event is not null)
			{
				// If this button already has an Event, it will be deleted from the action beforehand, overwriting it with the new event
				InputMap.ActionEraseEvent(action, Event);
			}

			InputMap.ActionAddEvent(action, @event);
		}

		Event = @event;

		StopWaitingForInput();
	}

	/// <summary>
	///		Signals that this button was clicked and is ready to receive an input to be used in the remap
	/// </summary>
	private void StartWaitingForInput()
	{
		var theme = GD.Load<StyleBoxFlat>("res://temp/remap_btn_active.tres");

		AddThemeStyleboxOverride("normal", theme);
		AddThemeStyleboxOverride("pressed", theme);
		AddThemeStyleboxOverride("hover", theme);

		EmitSignal(SignalName.ActionWasSelected, (int) InputType);
	}

	/// <summary>
	///		Signals that this button no longer should wait for an input, returning to it's original functioning
	/// </summary>
	public void StopWaitingForInput()
	{
		RemoveThemeStyleboxOverride("normal");
		RemoveThemeStyleboxOverride("pressed");
		RemoveThemeStyleboxOverride("hover");
	}
}
