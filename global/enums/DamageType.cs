namespace Epilogue.Global.Enums;
/// <summary>
///		Types of damage that can be caused to Actors and props
/// </summary>
public enum DamageType
{
	/// <summary>
	///		Damage caused by a bare-handed attack
	/// </summary>
	Unarmed,

	/// <summary>
	/// 	Damage caused by a thrown gun
	/// </summary>
	GunThrow,

	/// <summary>
	///		Damage caused by an attack that pierces the target
	/// </summary>
	Piercing,

	/// <summary>
	///		Damage caused by an attack that engulfs the target in flames
	/// </summary>
	Fire
}
