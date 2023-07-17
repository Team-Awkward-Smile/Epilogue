using Godot;
using System;

namespace Epilogue.ui;
public partial class PauseUI : Control
{
	public override void _Input(InputEvent @event)
	{
		if(@event.IsAction("pause_game") && @event.IsPressed())
		{
			Unpause();
		}
	}

	public override void _Ready()
	{
		GetNode<Slider>("VBoxContainer/HSlider2").CallDeferred("grab_focus");
		GetNode<Button>("VBoxContainer/CloseButton").Pressed += Unpause;
	}

	private void Unpause()
	{
		Hide();
		GetTree().Paused = false;

		GetViewport().SetInputAsHandled();
	}
}
