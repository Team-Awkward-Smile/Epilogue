using Epilogue.global.enums;
using Epilogue.input_icons;

using Godot;

namespace Epilogue.global.singletons;
/// <summary>
///		Singleton that controls inputs read from the player
/// </summary>
[Tool]
public partial class InputDeviceManager : Node
{
	/// <summary>
	///		Last type of input read, used to update the GUI with the correct textures
	/// </summary>
	public static InputTypeEnum? MostRecentInputType { get; set; }

	/// <summary>
	///		Deadzone of the left analog stick
	/// </summary>
	public float LeftJoystickDeadzone { get; set; } = 0.3f;

	/// <summary>
	///		Deadzone of the right analog stick
	/// </summary>
	public float RightJoystickDeadzone { get; set; } = 0.3f;

	private InputTypeEnum _newInputType;

	public override void _Input(InputEvent @event)
	{
		// If an input from an analog stick is read and it's below the deadzone, drop the input
		if(@event is InputEventJoypadMotion motionEvent)
		{
			if((motionEvent.Axis is JoyAxis.LeftX or JoyAxis.LeftY) && Mathf.Abs(motionEvent.AxisValue) < LeftJoystickDeadzone)
			{
				GetViewport().SetInputAsHandled();
				return;
			}
			else if((motionEvent.Axis is JoyAxis.RightX or JoyAxis.RightY) && Mathf.Abs(motionEvent.AxisValue) < RightJoystickDeadzone)
			{
				GetViewport().SetInputAsHandled();
				return;
			}
		}

		_newInputType = @event switch
		{
			InputEventJoypadButton => InputTypeEnum.Controller,
			InputEventJoypadMotion => InputTypeEnum.Controller,
			_ => InputTypeEnum.Keyboard
		};

		if(_newInputType != MostRecentInputType)
		{
			MostRecentInputType = _newInputType;

			// Calls every Node from the "InputType" group, telling them to run their respectives update routines
			GetTree().CallGroup("InputType", "InputTypeUpdate", (int) MostRecentInputType);
		}
	}

	public override void _Ready()
	{
		ProjectSettings.AddPropertyInfo(new Godot.Collections.Dictionary()
		{
			{ "name", "controls/controller_type" },
			{ "type", (int) Variant.Type.Int },
			{ "hint", (int) PropertyHint.Enum },
			{ "hint_string", "PlayStation,XBox,Nintendo Switch" }
		});
	}

	public static CompressedTexture2D GetKeyIcon(InputEvent @event)
	{
		if(@event is InputEventKey)
		{
			return PcIconMapper.GetIconForEvent(@event);
		}

		var brand = (InputDeviceBrand) ProjectSettings.GetSetting("controls/controller_type").AsInt16();

		return ControllerInputMapper.GetIconForEvent(brand, @event);
	}
}
