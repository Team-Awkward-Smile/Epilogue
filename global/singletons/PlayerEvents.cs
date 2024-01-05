using Epilogue.Global.Enums;

using Godot;

namespace Epilogue.Global.Singletons;
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

	/// <summary>
	///		Event triggered whenever the player dies
	/// </summary>
	[Signal] public delegate void PlayerDiedEventHandler();

	/// <summary>
	///		Event triggered whenever the Player takes damage
	/// </summary>
	/// <param name="damageTaken">The amount of damage taken</param>
	/// <param name="currentHP">The remaining HP value after taking damage</param>
	[Signal] public delegate void PlayerWasDamagedEventHandler(float damageTaken, float currentHP);

	/// <summary>
	///		Event triggered whenever the Player recovers HP
	/// </summary>
	/// <param name="healAmount">The amount of HP restored</param>
	/// <param name="currentHP">The remaining HP after being healed</param>
	[Signal] public delegate void PlayerWasHealedEventHandler(float healAmount, float currentHP);
}
