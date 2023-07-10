using Epilogue.global.enums;
using Godot;

namespace Epilogue.global.singletons;
public partial class InputDeviceManager : Node
{
	public static InputTypeEnum? MostRecentInputType { get; set; }
	public float LeftJoystickDeadzone { get; set; } = 0.3f;
	public float RightJoystickDeadzone { get; set; } = 0.3f;

	private InputTypeEnum _newInputType;

	public override void _Input(InputEvent @event)
	{
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

			GetTree().CallGroup("InputType", "InputTypeUpdate", (int) MostRecentInputType);
		}
	}
}
