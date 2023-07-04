using Epilogue.global.enums;
using Epilogue.global.singletons;
using Godot;
using Godot.Collections;

namespace Epilogue.actors.hestmor.aim;
public partial class Aim : Node
{
	private readonly Dictionary<AimDirectionEnum, int> _aimAngles = new()
	{
		{ AimDirectionEnum.Left | AimDirectionEnum.None, 180 },		// Left
		{ AimDirectionEnum.Right | AimDirectionEnum.None, 0 },		// Right
		{ AimDirectionEnum.None | AimDirectionEnum.Up, -90 },		// Up
		{ AimDirectionEnum.None | AimDirectionEnum.Down, 90 },		// Down

		{ AimDirectionEnum.Left | AimDirectionEnum.Up, -135 },		// Upper Left
		{ AimDirectionEnum.Left | AimDirectionEnum.Down, 135 },		// Lower Left
		{ AimDirectionEnum.Right | AimDirectionEnum.Up, -45 },		// Upper Right
		{ AimDirectionEnum.Right | AimDirectionEnum.Down, 45 }		// Lower Right
	};

	/// <summary>
	///		Method that runs whenever the Input Type changes
	/// </summary>
	private void InputTypeUpdate(InputTypeEnum inputType)
	{
		var mouseAim = GetChild(0);
		var buttonAim = GetChild(1);
		var stickAim = GetChild(2);

		mouseAim.ProcessMode = inputType == InputTypeEnum.Keyboard && Settings.ControlScheme == ControlSchemeEnum.Modern ? ProcessModeEnum.Inherit : ProcessModeEnum.Disabled;
		buttonAim.ProcessMode = Settings.ControlScheme == ControlSchemeEnum.Retro ? ProcessModeEnum.Inherit : ProcessModeEnum.Disabled;
		stickAim.ProcessMode = inputType == InputTypeEnum.Controller && Settings.ControlScheme == ControlSchemeEnum.Modern ? ProcessModeEnum.Inherit : ProcessModeEnum.Disabled;
	}

	public void SetAimDirection(AimDirectionEnum direction)
	{
		GetNode<Sprite2D>("../AimArrow").RotationDegrees = _aimAngles[direction];
	}
}
