using System;

namespace Epilogue.global.enums;
/// <summary>
///		Direction the character is currently aiming. This is a flag enum, so "top left", for example, will be "Up" + "Left"
/// </summary>
[Flags]
public enum AimDirectionEnum
{
	None = 0,
	Left = 1,
	Right = 1 << 1,
	Up = 1 << 2,
	Down = 1 << 3
}
