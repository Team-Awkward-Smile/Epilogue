using Epilogue.constants;
using Epilogue.global.enums;
using Epilogue.global.singletons;
using Epilogue.ui.popup;

using Godot;

using System;
using System.Collections.Generic;
using System.Linq;

namespace Epilogue.ui.remap;
public partial class RemapButton : Button
{
	public bool CanReadModernInputs { get; set; } = true;

	public InputTypeEnum InputType { get; set; }

	public List<string> Actions { get; set; } = new();

	public RemapButtonType ButtonType { get; set; }

	[Signal] public delegate void ActionWasSelectedEventHandler(InputTypeEnum inputType);

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

			if(_event is null)
			{
				Text = "Unbound";

				AddThemeColorOverride("font_color", new Color(0.47f, 0.47f, 0.47f));
			}
			else if(_event is InputEventKey key)
			{
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
					Text = _event.AsText() + "\n[Icon Pending]";
				}
				else
				{
					Icon = InputDeviceManager.GetKeyIcon(_event);
				}
			}
		}
	}

	private readonly List<Type> _validInputTypes = new();
	private InputEvent _event = null;

	public override void _Ready()
	{
		SetValidInputs();
		ButtonDown += StartWaitingForInput;

		CustomMinimumSize = new Vector2(128f, 128f);
	}

	private void SetValidInputs()
	{
		_validInputTypes.Clear();

		CanReadModernInputs = Settings.ControlScheme == ControlSchemeEnum.Modern;

		if(InputType == InputTypeEnum.PC)
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

	private void UpdateIconAndText(bool useDefaultActions)
	{
		var actionName = Actions.First();

		if(useDefaultActions)
		{
			actionName += $"_{Settings.ControlScheme.ToString().ToLower()}";
		}

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

	public void UpdateMapping(InputEvent @event)
	{
		foreach(var action in Actions)
		{
			if(Event is not null)
			{
				InputMap.ActionEraseEvent(action, Event);
			}

			InputMap.ActionAddEvent(action, @event);
		}

		Event = @event;

		StopWaitingForInput();
	}

	private void StartWaitingForInput()
	{
		var theme = GD.Load<StyleBoxFlat>("res://temp/remap_btn_active.tres");

		AddThemeStyleboxOverride("normal", theme);
		AddThemeStyleboxOverride("pressed", theme);
		AddThemeStyleboxOverride("hover", theme);

		EmitSignal(SignalName.ActionWasSelected, (int) InputType);
	}

	public void StopWaitingForInput()
	{
		RemoveThemeStyleboxOverride("normal");
		RemoveThemeStyleboxOverride("pressed");
		RemoveThemeStyleboxOverride("hover");
	}
}
