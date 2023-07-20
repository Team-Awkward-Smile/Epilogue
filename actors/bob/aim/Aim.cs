using Epilogue.global.enums;
using Epilogue.global.singletons;
using Epilogue.nodes;
using Godot;
using Godot.Collections;

namespace Epilogue.actors.hestmor.aim;
public partial class Aim : Node
{
	[Signal] public delegate void AimAngleUpdatedEventHandler(int angleDegrees);

	public int AimAngle { get; private set; }

	private Actor _actor;

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

	public override void _Ready()
	{
		_actor = (Actor) Owner;
	}

	public void SetAimDirection(AimDirectionEnum direction)
	{
		if(direction == AimDirectionEnum.None)
		{
			direction = _actor.FacingDirection == ActorFacingDirectionEnum.Left ? AimDirectionEnum.Left : AimDirectionEnum.Right;
		}

		if((direction & AimDirectionEnum.Left) != 0)
		{
			_actor.SetFacingDirection(ActorFacingDirectionEnum.Left);
		}
		else if((direction & AimDirectionEnum.Right) != 0)
		{
			_actor.SetFacingDirection(ActorFacingDirectionEnum.Right);
		}

		AimAngle = _aimAngles[direction];

		EmitSignal(SignalName.AimAngleUpdated, AimAngle);
	}
}
