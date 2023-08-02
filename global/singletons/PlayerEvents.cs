using Epilogue.global.enums;

using Godot;

namespace Epilogue.global.singletons;
/// <summary>
///		Global event emitter for events related to the player character
/// </summary>
public partial class PlayerEvents : GlobalEvents
{
	/// <summary>
	///		Event triggered whenever the game is waiting for the player to select an Execution speed
	/// </summary>
	[Signal] public delegate void StateAwaitingForExecutionSpeedEventHandler();

	/// <summary>
	///		Event triggered whenever the player select an Execution speed
	/// </summary>
	/// <param name="speed">Selected Execution speed</param>
	[Signal] public delegate void ExecutionSpeedSelectedEventHandler(ExecutionSpeed speed);
}
