using Godot;

namespace Epilogue.global.singletons;
/// <summary>
///		Global event emitter for events related to guns
/// </summary>
public partial class GunEvents : GlobalEvents
{
	/// <summary>
	///		Event triggered whenever the player picks up a gun
	/// </summary>
	/// <param name="currentAmmo">Current ammo count of the picked gun</param>
	/// <param name="maxAmmo">Maximum ammo count of the picked gun</param>
	[Signal] public delegate void PlayerPickedUpGunEventHandler(int currentAmmo, int maxAmmo);

	/// <summary>
	///		Event triggered whenever a gun is fired
	/// </summary>
	/// <param name="currentAmmo">The ammo count of the gun after firing</param>
	[Signal] public delegate void GunFiredEventHandler(int currentAmmo);

	/// <summary>
	///		Event triggered whenever a gun is dropped by the player
	/// </summary>
	[Signal] public delegate void GunWasDroppedEventHandler();
}
