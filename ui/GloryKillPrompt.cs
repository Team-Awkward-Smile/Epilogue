using Epilogue.Global.Enums;
using Epilogue.Global.Singletons;
using Epilogue.Util;
using Godot;

namespace Epilogue.ui;
/// <summary>
///		UI Screen responsible for displaying 2 options to the player and reading the answer
/// </summary>
public partial class GloryKillPrompt : Control
{
	/// <inheritdoc/>
	public override void _GuiInput(InputEvent @event)
	{
		if(@event.IsAction(InputUtils.GetInputActionName("execute_slow")) && @event.IsPressed())
		{
			GetNode<PlayerEvents>("/root/PlayerEvents").EmitGlobalSignal("ExecutionSpeedSelected", (int) ExecutionSpeed.Slow);
			Disable();
		}
		else if(@event.IsAction(InputUtils.GetInputActionName("execute_fast")) && @event.IsPressed())
		{
			GetNode<PlayerEvents>("/root/PlayerEvents").EmitGlobalSignal("ExecutionSpeedSelected", (int) ExecutionSpeed.Fast);
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
}
