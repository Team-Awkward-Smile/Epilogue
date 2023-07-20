using Epilogue.global.enums;
using Epilogue.global.singletons;
using Epilogue.util;
using Godot;

namespace Epilogue.ui;
public partial class GloryKillPrompt : Control
{
	public override void _Input(InputEvent @event)
	{
		if(@event.IsAction(InputUtils.GetInputActionName("execute_slow")) && @event.IsPressed())
		{
			GetNode<Events>("/root/Events").EmitGlobalSignal("GloryKillInputReceived", (int) GloryKillSpeed.Slow);
			Disable();
		}
		else if(@event.IsAction(InputUtils.GetInputActionName("execute_fast")) && @event.IsPressed())
		{
			GetNode<Events>("/root/Events").EmitGlobalSignal("GloryKillInputReceived", (int) GloryKillSpeed.Fast);
			Disable();
		}
	}

	public void Enable()
	{
		Visible = true;
		ProcessMode = ProcessModeEnum.Pausable;
	}

	public void Disable() 
	{
		Visible = false;
		ProcessMode = ProcessModeEnum.Disabled;
	}
}
