using Epilogue.global.enums;
using Epilogue.global.singletons;
using Godot;
using Godot.Collections;

namespace Epilogue.actors.hestmor.aim;
public partial class Aim : Node
{
	private readonly Dictionary<AimDirection, int> _aimAngles = new()
	{
		{ AimDirection.Left | AimDirection.None, 180 },		// Left
		{ AimDirection.Right | AimDirection.None, 0 },		// Right
		{ AimDirection.None | AimDirection.Up, -90 },		// Up
		{ AimDirection.None | AimDirection.Down, 90 },		// Down

		{ AimDirection.Left | AimDirection.Up, -135 },		// Upper Left
		{ AimDirection.Left | AimDirection.Down, 135 },		// Lower Left
		{ AimDirection.Right | AimDirection.Up, -45 },		// Upper Right
		{ AimDirection.Right | AimDirection.Down, 45 }		// Lower Right
	};

	public override void _Ready()
	{
	//	InputDeviceManager.InputTypeChanged += SetAimInputType;
	}

	private void SetAimInputType(InputType inputType)
	{
		switch(inputType)
		{

		}
	}

	public void SetAimDirection(AimDirection direction)
	{
		GetNode<Sprite2D>("../AimArrow").RotationDegrees = _aimAngles[direction];
	}
}
