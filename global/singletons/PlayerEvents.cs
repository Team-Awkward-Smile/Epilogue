using Epilogue.Global.Enums;
using Godot;

namespace Epilogue.Global.Singletons;
/// <summary>
///		Global event emitter for events related to the player character
/// </summary>
public partial class PlayerEvents : Node
{
	/// <summary>
	///		Event triggered whenever the game is waiting for the player to select an Execution speed
	/// </summary>
	[Signal] public delegate void QueryExecutionSpeedEventHandler();

	/// <summary>
	///		Event triggered whenever the player select an Execution speed
	/// </summary>
	/// <param name="speed">Selected Execution speed</param>
	[Signal] public delegate void ExecutionSpeedSelectedEventHandler(ExecutionSpeed speed);

	/// <summary>
	///		Signal emitted when the player's HP reaches 0 and they start dying
	/// </summary>
	[Signal] public delegate void PlayerIsDyingEventHandler();

	/// <summary>
	///		Event triggered whenever the player finishes dying
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

	/// <summary>
	///		Event triggered whenever the player character jumps
	/// </summary>
	[Signal] public delegate void PlayerJumpedEventHandler();
}
