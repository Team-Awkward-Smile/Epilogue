using Epilogue.global.enums;
using Godot;

namespace Epilogue.global.singletons;
public partial class Events : Node
{
	[Signal] public delegate void StateAwaitingForGloryKillInputEventHandler();
	[Signal] public delegate void GloryKillInputReceivedEventHandler(GloryKillSpeed speed);
	[Signal] public delegate void PlayerPickedUpGunEventHandler(int currentAmmo, int maxAmmo);
	[Signal] public delegate void GunFiredEventHandler(int currentAmmo);
	[Signal] public delegate void GunWasDroppedEventHandler();

	public void EmitGlobalSignal(StringName signalName, params Variant[] args)
	{
		EmitSignal(signalName, args);
		GD.Print($"Signal [{signalName}] emmitted with args [{string.Join(", ", args)}]");
	}
}
