using Godot;

namespace Epilogue.UI.Pause;
/// <summary>
///		Screen responsible for displaying the menu when the game is paused, and reading the input to un-pause it
/// </summary>
public partial class PauseUI : Nodes.UIScreen
{
	private CanvasLayer _pauseLayer;
	private CanvasLayer _settingsLayer;

	/// <inheritdoc/>
	public override void _Input(InputEvent @event)
	{
		if (@event.IsActionPressed("pause_game"))
		{
			Unpause();
		}
	}

	/// <inheritdoc/>
	public override void _Ready()
	{
		GetNode<Button>("PauseLayer/VBoxContainer/ResumeButton").Pressed += Unpause;
		GetNode<Button>("PauseLayer/VBoxContainer/SettingsButton").Pressed += ShowSettingsScreen;
		// TODO: 226 - Make the button work properly after we have a Main Menu
		GetNode<Button>("PauseLayer/VBoxContainer/QuitMenuButton").Pressed += () => GD.Print("There is no Main Menu yet :C");
		GetNode<Button>("PauseLayer/VBoxContainer/QuitDesktopButton").Pressed += () => GetTree().Quit();

		_pauseLayer = GetNode<CanvasLayer>("PauseLayer");
		_settingsLayer = GetNode<CanvasLayer>("SettingsLayer");

		_pauseLayer.Hide();
		_settingsLayer.Hide();
	}

	/// <inheritdoc/>
	public override void Enable(bool pauseTree = false)
	{
		_pauseLayer.Show();

		GetTree().Paused = true;
	}

	private void ShowSettingsScreen()
	{
		_pauseLayer.Hide();
		_settingsLayer.Show();
	}

	private void Unpause()
	{
		_pauseLayer.Hide();
		_settingsLayer.Hide();

		Disable(true);

		GetViewport().SetInputAsHandled();
	}
}
