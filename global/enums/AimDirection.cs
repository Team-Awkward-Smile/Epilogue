using System;

namespace Epilogue.global.enums;
/// <summary>
///		Direction the character is currently aiming. This is a flag enum, so "top left", for example, will be "Up" + "Left"
/// </summary>
[Flags]
public enum AimDirection
{
	/// <summary>
	///		No direction (0000)
	/// </summary>
	None = 0,

	/// <summary>
	///		Left (0001)
	/// </summary>
	Left = 1,

	/// <summary>
	///		Right (0010)
	/// </summary>
	Right = 1 << 1,

	/// <summary>
	///		Up (0100)
	/// </summary>
	Up = 1 << 2,

	/// <summary>
	///		Down (1000)
	/// </summary>
	Down = 1 << 3
}
