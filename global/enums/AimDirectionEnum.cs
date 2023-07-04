using System;

namespace Epilogue.global.enums;
[Flags]
public enum AimDirectionEnum
{
	None = 0,
	Left = 1,
	Right = 1 << 1,
	Up = 1 << 2,
	Down = 1 << 3
}
