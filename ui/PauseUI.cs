using Godot;
using System;

namespace Epilogue.ui;
/// <summary>
///		Screen responsible for displaying the menu when the game is paused, and reading the input to un-pause it
/// </summary>
public partial class PauseUI : Control
{
	/// <inheritdoc/>
	public override void _Input(InputEvent @event)
	{
		if(@event.IsAction("pause_game") && @event.IsPressed())
		{
			Unpause();
		}
	}

	/// <inheritdoc/>
	public override void _Ready()
	{
		GetNode<Slider>("VBoxContainer/HSlider2").CallDeferred("grab_focus");
		GetNode<Button>("VBoxContainer/CloseButton").Pressed += Unpause;
		GetNode<Button>("VBoxContainer/QuitButton").Pressed += () => GetTree().Quit();
	}

	private void Unpause()
	{
		Hide();
		GetTree().Paused = false;

		GetViewport().SetInputAsHandled();
	}
}
