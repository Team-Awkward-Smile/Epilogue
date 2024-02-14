using Epilogue.Global.Enums;
using Epilogue.Global.Singletons;
using Godot;
using System.Linq;

namespace Epilogue.UI;
/// <summary>
///		UI Screen responsible for displaying 2 options to the player and reading the answer
/// </summary>
public partial class GloryKillPrompt : Control
{
	/// <inheritdoc/>
	public override void _UnhandledInput(InputEvent @event)
	{
		if (@event.IsEcho())
		{
			return;
		}

		if (@event.IsActionPressed("execute_slow"))
		{
			GetNode<PlayerEvents>("/root/PlayerEvents").EmitSignal(PlayerEvents.SignalName.ExecutionSpeedSelected, (int)ExecutionSpeed.Slow);
			GetViewport().SetInputAsHandled();
			Disable();
		}
		else if (@event.IsActionPressed("execute_fast"))
		{
			GetNode<PlayerEvents>("/root/PlayerEvents").EmitSignal(PlayerEvents.SignalName.ExecutionSpeedSelected, (int)ExecutionSpeed.Fast);
			GetViewport().SetInputAsHandled();
			Disable();
		}
	}

	/// <summary>
	///		Enables this Screen
	/// </summary>
	public void Enable()
	{
		Visible = true;
		ProcessMode = ProcessModeEnum.Pausable;
	}

	/// <summary>
	///		Disables this Screen
	/// </summary>
	public void Disable()
	{
		Visible = false;
		ProcessMode = ProcessModeEnum.Disabled;
	}

	/// <inheritdoc/>
	public override void _Ready()
	{
		GetNode<Label>("%SlowExecutionLabel").Text = InputMap.ActionGetEvents("execute_slow").First().AsText() + "\nSlow";
		GetNode<Label>("%FastExecutionLabel").Text = InputMap.ActionGetEvents("execute_fast").First().AsText() + "\nFast";
	}
}
