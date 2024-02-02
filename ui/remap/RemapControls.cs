using Epilogue.Global.Enums;
using Epilogue.Global.Singletons;
using Epilogue.Nodes;
using Epilogue.UI.popup;
using Godot;
using System.Collections.Generic;
using System.Linq;

namespace Epilogue.UI.Remap;
/// <summary>
///		Screen to allow the player to remap the controls of the game.
///		Presents a list of actions, and allows new events from PC and controller to be mapped to them
/// </summary>
public partial class RemapControls : Screen
{
	private static readonly List<GroupAction> _moveActions = new()
	{
		new() { Label = "Move Left", Actions = new() { "move_left" } },
		new() { Label = "Move Right", Actions = new() { "move_right" } },
		new() { Label = "Run", Actions = new() { "toggle_run" } },
		new() { Label = "Slide", Actions = new() { "slide" } },
		new() { Label = "Cancel Slide/Look Up", Actions = new() { "cancel_slide", "look_up" } },
		new() { Label = "Crouch/Look Down", Actions = new() { "crouch" } },
		new() { Label = "Jump/Vault/Climb Ledge", Actions = new() { "jump" } }
	};

	private static readonly List<GroupAction> _combatActions = new()
	{
		new() { Label = "Attack/Shoot", Actions = new() { "melee", "shoot" } },
		new() { Label = "Growl/Interact", Actions = new() { "growl", "interact" } }
	};

	private static readonly List<GroupAction> _uiActions = new()
	{
		new() { Label = "Pause/Unpause", Actions = new() { "pause" } }
	};

	private readonly Dictionary<StringName, List<InputEvent>> _originalMapping = new();

	private bool _hasUnsavedChanges = false;
	private CustomPopup _instructionsPopup;
	private InputDeviceType _validPopupInput;
	private RemapButton _selectedButton;
	private ControlScheme _originalScheme;

	/// <inheritdoc/>
	public override void _Ready()
	{
		AddActionRow(GetNode<GridContainer>("ScrollContainer/Control/MovementActions"), _moveActions);
		AddActionRow(GetNode<GridContainer>("ScrollContainer/Control/CombatActions"), _combatActions);
		AddActionRow(GetNode<GridContainer>("ScrollContainer/Control/UIActions"), _uiActions);

		var actions = _moveActions.Union(_combatActions).Union(_uiActions).ToList();

		foreach (var actionGroup in actions)
		{
			foreach (var action in actionGroup.Actions)
			{
				_originalMapping.Add(action, InputMap.ActionGetEvents(action).ToList());
			}
		}

		GetNode<Button>("Save").ButtonDown += SaveMapping;
		GetNode<Button>("%Default").ButtonDown += ResetToDefault;

		var optionButton = GetNode<OptionButton>("%OptionButton");

		optionButton.ItemSelected += UpdateControlScheme;
		optionButton.Select((int)Settings.ControlScheme);

		_originalScheme = (ControlScheme)optionButton.Selected;

		_instructionsPopup = CustomPopup.NewCustomPopup();
		_instructionsPopup.DialogText = "Press any key (Esc to cancel)";
		_instructionsPopup.GetOkButton().Hide();
		_instructionsPopup.WindowInput += OnRemapEventReceived;

		AddChild(_instructionsPopup);

		Draw += () =>
		{
			GetTree().CallGroup("remap_buttons", "UpdateIconAndText");
		};
	}

	/// <summary>
	///		Logic executed whenever an action is being remapped and a valid input is detected
	/// </summary>
	/// <param name="event">The input detected</param>
	private void OnRemapEventReceived(InputEvent @event)
	{
		// Only allows press inputs (and not release inputs)
		if (@event.IsReleased())
		{
			return;
		}

		// Pressing Esc will cancel the remap
		// TODO: 36 - Improve this logic and allow it to work with controllers as well
		if (@event.IsAction("ui_cancel") && @event is InputEventKey)
		{
			_instructionsPopup.Hide();
			_selectedButton.StopWaitingForInput();
			return;
		}

		// Filters out any invalid input depending of the button clicked
		if (_validPopupInput == InputDeviceType.PC && @event is not (InputEventKey or InputEventMouseButton))
		{
			return;
		}
		else if (_validPopupInput == InputDeviceType.Controller && @event is not (InputEventJoypadButton or InputEventJoypadMotion))
		{
			return;
		}
		else if (@event is InputEventJoypadMotion motion && Mathf.Abs(motion.AxisValue) < 0.8f)
		{
			// Joystick movement inputs are only detected if they're above 0.8 in strength
			return;
		}

		_instructionsPopup.Hide();

		// Checks if the detected event is already mapped to one or more actions
		if (IsEventAlreadyMapped(@event, out var existingActions))
		{
			var confirmDialog = CustomPopup.NewCustomPopup();

			// Popup warning the player the detected input is already in use
			confirmDialog.DialogText = $"Key already mapped to:\n- {string.Join("\n- ", existingActions)}";
			confirmDialog.OkButtonText = "Replace";
			confirmDialog.AddCancelButton("Cancel");

			// Player decided to overwrite the other actions
			confirmDialog.GetOkButton().Pressed += () =>
			{
				foreach (var action in existingActions)
				{
					InputMap.ActionEraseEvent(action, @event);
				}

				AssignEventToCurrentActions(@event);

				// Calls the method "UpdateIconAndText" of every RemapButton in the screen to update their icons/text
				GetTree().CallGroup("remap_buttons", "UpdateIconAndText");
			};

			// Player decided to NOT overwrite the other actions
			confirmDialog.Canceled += () =>
			{
				_selectedButton.StopWaitingForInput();
			};

			AddChild(confirmDialog);

			confirmDialog.PopupCentered();
		}
		else
		{
			AssignEventToCurrentActions(@event);
		}
	}

	/// <summary>
	///		Add the detected event to the corresponsing action(s)
	/// </summary>
	/// <param name="event">The event to be added</param>
	private void AssignEventToCurrentActions(InputEvent @event)
	{
		_selectedButton.UpdateMapping(@event);
		_hasUnsavedChanges = true;
	}

	/// <summary>
	///		Saves the changes done to disk, allowing them to be used later
	/// </summary>
	private void SaveMapping()
	{
		Settings.SaveSettings();

		_hasUnsavedChanges = false;

		GetTree().CallGroup("InputReadingUpdatesSubscribers", "UpdateInputReading");
	}

	/// <summary>
	///		Resets the mapping to their default value, undoing every change made by the player
	/// </summary>
	private void ResetToDefault()
	{
		var confirmDialog = CustomPopup.NewCustomPopup();

		confirmDialog.DialogText = "Are you sure you want to reset the controls?";
		confirmDialog.AddCancelButton("No");

		var okButton = confirmDialog.GetOkButton();

		okButton.Text = "Yes";
		okButton.Pressed += () =>
		{
			InputMap.LoadFromProjectSettings();

			var actions = _moveActions.Union(_combatActions).Union(_uiActions).ToList();

			foreach (var actionGroup in actions)
			{
				foreach (var action in actionGroup.Actions)
				{
					var defaultAction = InputMap.ActionGetEvents($"{action}_{Settings.ControlScheme.ToString().ToLower()}");

					foreach (var defaultEvent in defaultAction)
					{
						InputMap.ActionAddEvent(action, defaultEvent);
					}
				}
			}

			SaveMapping();
			GetTree().CallGroup("remap_buttons", "UpdateIconAndText");
		};

		AddChild(confirmDialog);

		confirmDialog.PopupCentered();
	}

	/// <summary>
	///		Changes the current Control Scheme and updates every button on-screen. 
	///		This method runs automatically whenever the OptionButton is used to change the current Control Scheme
	/// </summary>
	/// <param name="index">The index of the selected item</param>
	private void UpdateControlScheme(long index)
	{
		// Whenever the player changes the Control Scheme, the mapping will reset to the default of the selected scheme
		Settings.ControlScheme = (ControlScheme)index;

		var controlScheme = Settings.ControlScheme.ToString().ToLower();
		var defaultActions = _moveActions.Union(_combatActions).Union(_uiActions).ToList();

		InputMap.LoadFromProjectSettings();

		foreach (var actionGroup in defaultActions)
		{
			foreach (var action in actionGroup.Actions)
			{
				var defaultAction = action + $"_{controlScheme}";

				foreach (var @event in InputMap.ActionGetEvents(defaultAction))
				{
					InputMap.ActionAddEvent(action, @event);
				}
			}
		}

		// Calls the method "UpdateIconAndText" of every RemapButton, to make them update their icon/text
		GetTree().CallGroup("remap_buttons", "UpdateIconAndText");

		_hasUnsavedChanges = true;
	}

	/// <summary>
	///		Adds a new row to the screen, with a Label containing the actions, and 3 buttons used to remap them
	/// </summary>
	/// <param name="parent">The GridContainer that will be used as the parent of every row</param>
	/// <param name="actions">A Dictionary containing a string with the name of the action(s), and a List of strings with the name of each individual action that will be used with the InputMap</param>
	private void AddActionRow(GridContainer parent, List<GroupAction> actions)
	{
		foreach (var actionGroup in actions)
		{
			var events = InputMap.ActionGetEvents(actionGroup.Actions.First());

			if (events.Count == 0)
			{
				events = InputMap.ActionGetEvents(actionGroup.Actions.First() + $"_{Settings.ControlScheme.ToString().ToLower()}");

				foreach (var action in actionGroup.Actions)
				{
					foreach (var e in events)
					{
						InputMap.ActionAddEvent(action, e);
					}
				}
			}

			var pcEvents = events.Where(e => e is InputEventKey or InputEventMouse);

			var label = new Label()
			{
				SizeFlagsHorizontal = SizeFlags.ExpandFill,
				Text = actionGroup.Label
			};

			var primaryButton = new RemapButton()
			{
				SizeFlagsHorizontal = SizeFlags.ExpandFill,
				InputType = InputDeviceType.PC,
				Event = pcEvents.FirstOrDefault(),
				Actions = actionGroup.Actions,
				ButtonType = RemapButtonType.PcPrimary
			};

			var secondaryButton = new RemapButton()
			{
				SizeFlagsHorizontal = SizeFlags.ExpandFill,
				InputType = InputDeviceType.PC,
				Event = pcEvents.Skip(1).FirstOrDefault(),
				Actions = actionGroup.Actions,
				ButtonType = RemapButtonType.PcSecondary
			};

			var controllerButton = new RemapButton()
			{
				SizeFlagsHorizontal = SizeFlags.ExpandFill,
				InputType = InputDeviceType.Controller,
				Event = events.Where(e => e is InputEventJoypadButton or InputEventJoypadMotion).FirstOrDefault(),
				Actions = actionGroup.Actions,
				ButtonType = RemapButtonType.Controller
			};

			parent.AddChild(label);

			foreach (var remapButton in new List<RemapButton>() { primaryButton, secondaryButton, controllerButton })
			{
				remapButton.AddToGroup("remap_buttons");
				remapButton.ActionWasSelected += (InputDeviceType inputType) =>
				{
					_selectedButton = remapButton;
					_validPopupInput = inputType;
					_instructionsPopup.PopupCentered();
				};

				parent.AddChild(remapButton);
			}
		}
	}

	/// <summary>
	///		Checks if the InputEvent is already mapped to any other action
	/// </summary>
	/// <param name="event">The event to be validated</param>
	/// <param name="existingActions">The list of actions that are already mapped to that event</param>
	/// <returns><c>true</c>, if at least 1 action is already mapped to the event (in this case, <paramref name="existingActions"/> will contain those actions); <c>false</c>, otherwise</returns>
	private static bool IsEventAlreadyMapped(InputEvent @event, out List<string> existingActions)
	{
		var actions = _combatActions.Union(_moveActions).Union(_uiActions).ToList();
		var actionsWithMapping = new List<string>();

		foreach (var actionGroup in actions)
		{
			foreach (var action in actionGroup.Actions)
			{
				if (InputMap.EventIsAction(@event, action, true))
				{
					actionsWithMapping.Add(action);
				}
			}
		}

		existingActions = actionsWithMapping;

		return existingActions.Count > 0;
	}

	/// <inheritdoc/>
	public override void Close(bool unpauseTree = false)
	{
		// If any action is not mapped to any key, the Screen cannot be closed
		if (!ValidateEmptyActions())
		{
			return;
		}

		if (_hasUnsavedChanges)
		{
			OnUnsavedChangesClose();
		}
		else
		{
			base.Close(true);
		}
	}

	/// <summary>
	///		Checks if any action displayed on the list is not mapped to anything
	/// </summary>
	/// <returns><c>true</c>, if every action has at least 1 event mapped to it; <c>false</c>, otherwise</returns>
	private bool ValidateEmptyActions()
	{
		var actions = _moveActions.Union(_uiActions).Union(_combatActions).ToList();
		var emptyActions = new List<string>();

		foreach (var actionGroup in actions)
		{
			foreach (var action in actionGroup.Actions)
			{
				var events = InputMap.ActionGetEvents(action);

				if (events.Count == 0)
				{
					emptyActions.Add(action);
				}
			}
		}

		if (emptyActions.Count > 0)
		{
			var popup = CustomPopup.NewCustomPopup();

			popup.DialogText = $"The following actions are not mapped to any key:\n- {string.Join("\n- ", emptyActions)}";

			AddChild(popup);

			popup.PopupCentered();

			return false;
		}

		return true;
	}

	/// <summary>
	///		Displays a popup when the player tries to exit the screen with unsaved changed present
	/// </summary>
	private void OnUnsavedChangesClose()
	{
		var confirmPopup = GD.Load<PackedScene>("res://ui/popup/custom_popup.tscn").Instantiate() as CustomPopup;

		confirmPopup.DialogText = "You have unsaved changes";
		confirmPopup.OkButtonText = "Save & Exit";
		confirmPopup.CustomButtons = new()
		{
			new()
			{
				ButtonText = "Exit Anyway",
				Action = "exit_no_save"
			},
			new()
			{
				ButtonText = "Cancel",
				Action = "cancel"
			}
		};

		confirmPopup.GetOkButton().Pressed += () =>
		{
			SaveMapping();
			base.Close(true);
		};

		confirmPopup.CustomAction += (StringName action) =>
		{
			confirmPopup.Hide();

			if (action == "exit_no_save")
			{
				foreach (var mapping in _originalMapping)
				{
					InputMap.ActionEraseEvents(mapping.Key);

					foreach (var @event in mapping.Value)
					{
						InputMap.ActionAddEvent(mapping.Key, @event);
					}
				}

				Settings.ControlScheme = _originalScheme;

				base.Close(true);
			}
		};

		AddChild(confirmPopup);

		confirmPopup.PopupCentered();
	}
}
