using Epilogue.nodes;

using Godot;

using System.Collections.Generic;
using System.Linq;

namespace Epilogue.ui;
public partial class RemapControls : UI
{
	private readonly List<string> _actions = new()
	{
		"move_left",
		"move_right",
		"jump",
		"crouch",
		"look_up",
		"slide",
		"melee",
		"pickup_gun",
		"growl",
		"shoot",
		"cancel_slide"
	};

	private List<Control> _nodes = new();

	public override void _Ready()
	{
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
			var actionModern = InputMap.ActionGetEvents($"{actionName}_modern");
			var actionRetro = InputMap.ActionGetEvents($"{actionName}_retro");
			var button1Modern = new RemapButton();
			var button2Modern = new RemapButton();
			var button1Retro = new RemapButton();
			var button2Retro = new RemapButton();
			var label = new Label()
			{
				Text = actionName,
				HorizontalAlignment = HorizontalAlignment.Right,
			};

			var pcActionModern = actionModern.Where(a => a is InputEventKey or InputEventMouseButton).ToArray();
			var pcActionRetro = actionRetro.Where(a => a is InputEventKey).ToArray();

			button1Modern.ActionName = actionName + "_modern";
			button1Modern.InputEvent = pcActionModern[0];
			button2Modern.ActionName = actionName + "_modern";

			button1Retro.ActionName = actionName + "_retro";
			button1Retro.InputEvent = pcActionRetro[0];
			button2Retro.ActionName = actionName + "_retro";

			if(pcActionModern.Length > 1)
			{
				button2Modern.InputEvent = pcActionModern[1];
			}

			if(pcActionRetro.Length > 1)
			{
				button2Retro.InputEvent = pcActionRetro[1];
			}

			modernGrid.AddChild(label);
			modernGrid.AddChild(button1Modern);
			modernGrid.AddChild(button2Modern);

			var label2 = label.Duplicate() as Label;

			retroGrid.AddChild(label2);
			retroGrid.AddChild(button1Retro);
			retroGrid.AddChild(button2Retro);

			_nodes.AddRange(new List<Control>
			{
				label, label2, button1Modern, button2Modern, button1Retro, button2Retro
			});
		}
	}
}
