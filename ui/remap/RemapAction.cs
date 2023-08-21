using Epilogue.global.enums;
using Epilogue.global.singletons;

using Godot;

using System.Collections.Generic;
using System.Linq;

namespace Epilogue.ui;
public partial class RemapAction : GridContainer
{
	public override void _Ready()
	{
		GetNode<OptionButton>("../OptionButton").ItemSelected += (long index) => UpdateList((ControlSchemeEnum) index);
	}

	private void UpdateList(ControlSchemeEnum controlScheme)
	{
		foreach(var row in _actions)
		{
			var events = InputMap.ActionGetEvents(row.Actions.First());

			if(events.Count == 0)
			{
				events = InputMap.ActionGetEvents(row.Actions.First() + $"_{controlScheme.ToString().ToLower()}");
			}

			var primaryEvent = events[0];
			var controllerEvent = events[1];

			if(primaryEvent is InputEventKey key)
			{
				var unicodeChar = ((char) key.Unicode).ToString();

				if(unicodeChar != "\0" && unicodeChar != " ")
				{
					row.PrimaryButton.Text = unicodeChar.ToUpper();
				}
				else
				{
					row.PrimaryButton.Text = key.PhysicalKeycode.ToString();
				}
			}
			else
			{
				row.PrimaryButton.Text = primaryEvent.AsText() + "\n[Icon Pending]";
			}

			row.ControllerButton.Icon = InputDeviceManager.GetKeyIcon(controllerEvent);
			row.ControllerButton.IconAlignment = HorizontalAlignment.Center;

			row.ControllerButton.Set("theme_override_constants/icon_max_width", 70f);
		}
	}

	private List<ActionRow> _actions = new();

	public void AddActionRow(Dictionary<string, List<string>> actions)
	{
		foreach(var action in actions)
		{
			var row = new ActionRow()
			{
				Label = new()
				{
					SizeFlagsHorizontal = SizeFlags.ExpandFill,
					Text = action.Key
				},
				PrimaryButton = new()
				{
					SizeFlagsHorizontal = SizeFlags.ExpandFill
				},
				SecondaryButton = new()
				{
					SizeFlagsHorizontal = SizeFlags.ExpandFill
				},
				ControllerButton = new() 
				{ 
					SizeFlagsHorizontal = SizeFlags.ExpandFill
				},
				Actions = action.Value
			};

			AddChild(row.Label);
			AddChild(row.PrimaryButton);
			AddChild(row.SecondaryButton);
			AddChild(row.ControllerButton);

			_actions.Add(row);

			UpdateList((ControlSchemeEnum) GetNode<OptionButton>("../OptionButton").Selected);
		}
	}

	private class ActionRow
	{
		public Label Label { get; set; }
		public Button PrimaryButton { get; set; }
		public Button SecondaryButton { get; set;}
        public Button ControllerButton { get; set; }
        public List<string> Actions { get; set; }
    }
}
