using Epilogue.Global.Enums;
using Epilogue.Global.Singletons;
using Epilogue.Nodes;
using Godot;
using Godot.Collections;

namespace Epilogue.Actors.Hestmor.aim;
/// <summary>
///		Node responsible for handling the aiming inputs and setting the correct angles
/// </summary>
public partial class Aim : Node
{
	/// <summary>
	///		Event fired every time the aiming angle is changed
	/// </summary>
	/// <param name="angleDegrees">The new angles, in degrees</param>
	[Signal] public delegate void AimAngleUpdatedEventHandler(int angleDegrees);

	/// <summary>
	///		Current angle Hestmor is aiming at, in degrees
	/// </summary>
	public int AimAngle { get; private set; }

	private Actor _actor;

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

	/// <summary>
	///		Method that runs whenever the Input Type changes
	/// </summary>
	private void UpdateInputReading()
	{
		var inputType = InputDeviceManager.MostRecentInputType ?? InputDeviceType.PC;
		var mouseAim = GetChild(0);
		var buttonAim = GetChild(1);
		var stickAim = GetChild(2);

		mouseAim.ProcessMode = inputType == InputDeviceType.PC && Settings.ControlScheme == ControlScheme.Modern ? ProcessModeEnum.Inherit : ProcessModeEnum.Disabled;
		buttonAim.ProcessMode = Settings.ControlScheme == ControlScheme.Retro ? ProcessModeEnum.Inherit : ProcessModeEnum.Disabled;
		stickAim.ProcessMode = inputType == InputDeviceType.Controller && Settings.ControlScheme == ControlScheme.Modern ? ProcessModeEnum.Inherit : ProcessModeEnum.Disabled;
	}

	/// <inheritdoc/>
	public override void _Ready()
	{
		_actor = (Actor) Owner;
		UpdateInputReading();
	}

	/// <summary>
	///		Sets a new angle to be used by guns. The informed directions are converted into the corresponsing angles
	/// </summary>
	/// <param name="direction">The flags representing the new direction Hestmo is aiming at</param>
	public void SetAimDirection(AimDirection direction)
	{
		if(direction == AimDirection.None)
		{
			direction = _actor.FacingDirection == ActorFacingDirection.Left ? AimDirection.Left : AimDirection.Right;
		}

		if((direction & AimDirection.Left) != 0)
		{
			_actor.SetFacingDirection(ActorFacingDirection.Left);
		}
		else if((direction & AimDirection.Right) != 0)
		{
			_actor.SetFacingDirection(ActorFacingDirection.Right);
		}

		AimAngle = _aimAngles[direction];

		EmitSignal(SignalName.AimAngleUpdated, AimAngle);
	}
}
