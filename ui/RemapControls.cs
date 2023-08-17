using Epilogue.nodes;

using Godot;

using System.Collections.Generic;
using System.Linq;

namespace Epilogue.ui;
public partial class RemapControls : UI
{
	private List<Control> _nodes = new();
	private List<StringName> _actions = new();

	public override void _Ready()
	{
		// Gets every action that's not built-in (built-in actions start with "ui_" in their names)
		_actions = InputMap.GetActions().Where(a => !a.ToString().StartsWith("ui_") && !a.ToString().Contains("controller")).ToList();

		GetNode<Button>("%Return").ButtonDown += () =>
		{
			GetTree().Paused = false;
			QueueFree();
		};

		GetNode<Button>("%Default").ButtonDown += () =>
		{
			InputMap.LoadFromProjectSettings();

			foreach(var button in _nodes.ToList())
			{
				button.QueueFree();
			}

			UpdateButtonLabel();
		};

		UpdateButtonLabel();
	}

	private void UpdateButtonLabel()
	{
		_nodes.Clear();

		var modernGrid = GetNode<GridContainer>("%Modern");
		var retroGrid = GetNode<GridContainer>("%Retro");

		foreach(var actionName in _actions)
		{
			var action = InputMap.ActionGetEvents(actionName);
			var scene = GD.Load<PackedScene>("res://ui/remap_button.tscn");
			var button1 = scene.Instantiate() as RemapButton;
			var button2 = scene.Instantiate() as RemapButton;
			var label = new Label()
			{
				Text = actionName,
				HorizontalAlignment = HorizontalAlignment.Right,
				Size = new Vector2(15f, 50f)
			};

			// Adds the object to the correct screen, depending on the Control Scheme of the action
			if(actionName.ToString().EndsWith("modern"))
			{
				modernGrid.AddChild(label);
				modernGrid.AddChild(button1);
				modernGrid.AddChild(button2);
			}
			else
			{
				retroGrid.AddChild(label);
				retroGrid.AddChild(button1);
				retroGrid.AddChild(button2);
			}

			button1.ActionName = actionName;
			button1.InputEvent = action[0];
			button2.ActionName = actionName;

			// Sets the secondary button if the action has more than 1 key mapped to it
			if(action.Count > 1)
			{
				button2.InputEvent = action[1];
			}

			_nodes.AddRange(new List<Control>
			{
				label, button1, button2
			});
		}
	}
}
