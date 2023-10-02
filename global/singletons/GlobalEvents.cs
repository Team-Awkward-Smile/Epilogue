using Godot;

namespace Epilogue.global.singletons;
/// <summary>
///		Base class for Global Event Emitters
/// </summary>
public partial class GlobalEvents : Node
{
	/// <summary>
	///		Emits the requested signal
	/// </summary>
	/// <param name="signalName">Name of the signal to be emitted</param>
	/// <param name="args">Parameters to be emitted alongside the signal (0 or more parameters)</param>
	public void EmitGlobalSignal(StringName signalName, params Variant[] args)
	{
		GD.Print($"[{Name}] - Emiting [{signalName}] with args [{string.Join(", ", args)}]");
		EmitSignal(signalName, args);
	}
}
