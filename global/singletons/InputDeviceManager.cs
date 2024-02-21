using Epilogue.Global.Enums;
using Epilogue.InputIcons;
using Godot;

namespace Epilogue.Global.Singletons;
/// <summary>
///		Singleton that controls inputs read from the player
/// </summary>
[Tool]
public partial class InputDeviceManager : Node
{
	/// <summary>
	///		Last type of input read, used to update the GUI with the correct textures
	/// </summary>
	public static InputDeviceType? MostRecentInputType { get; set; }

	/// <summary>
	///		Deadzone of the left analog stick
	/// </summary>
	public float LeftJoystickDeadzone { get; set; } = 0.3f;

	/// <summary>
	///		Deadzone of the right analog stick
	/// </summary>
	public float RightJoystickDeadzone { get; set; } = 0.3f;

	private InputDeviceType _newInputType;

	/// <inheritdoc/>
	public override void _Input(InputEvent @event)
	{
		// If an input from an analog stick is read and it's below the deadzone, drop the input
		if (@event is InputEventJoypadMotion motionEvent)
		{
			if ((motionEvent.Axis is JoyAxis.LeftX or JoyAxis.LeftY) && Mathf.Abs(motionEvent.AxisValue) < LeftJoystickDeadzone)
			{
				GetViewport().SetInputAsHandled();
				return;
			}
			else if ((motionEvent.Axis is JoyAxis.RightX or JoyAxis.RightY) && Mathf.Abs(motionEvent.AxisValue) < RightJoystickDeadzone)
			{
				GetViewport().SetInputAsHandled();
				return;
			}
		}

		_newInputType = @event switch
		{
			InputEventJoypadButton or InputEventJoypadMotion => InputDeviceType.Controller,
			_ => InputDeviceType.PC
		};

		if (_newInputType != MostRecentInputType)
		{
			MostRecentInputType = _newInputType;

			// Calls every Node from the "InputReadingUpdatesSubscribers" group, telling them to run their respectives update routines
			GetTree().CallGroup("InputReadingUpdatesSubscribers", "UpdateInputReading");
		}
	}

	/// <summary>
	///		Maps an InputEvent to it's corresponding icon. Used to display events as icons on-screen, regardless of the original event
	/// </summary>
	/// <param name="event">Event to be mapped (must be a <see cref="InputEventMouseButton"/>, <see cref="InputEventJoypadButton"/>, or <see cref="InputEventJoypadMotion"/>)</param>
	/// <returns>A CompressedTexture2D of the corresponding icon, based on the InputEvent and the brand of controller selected by the player</returns>
	public static CompressedTexture2D GetKeyIcon(InputEvent @event)
	{
		if (@event is InputEventKey)
		{
			return PcIconMapper.GetIconForEvent(@event);
		}

		var brand = (InputDeviceBrand)ProjectSettings.GetSetting("epilogue/controls/controller_type").AsInt16();

		return ControllerInputMapper.GetIconForEvent(brand, @event);
	}
}
