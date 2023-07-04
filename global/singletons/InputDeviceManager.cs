using Epilogue.global.enums;
using Godot;

namespace Epilogue.global.singletons;
public partial class InputDeviceManager : Node
{
	public static InputType MostRecentInputType { get; set; }
	public float LeftJoystickDeadzone { get; set; } = 0.1f;
	public float RightJoystickDeadzone { get; set; } = 0.1f;

	private InputType? _oldInputType = null;

	[Signal] public delegate void InputTypeChangedEventHandler(InputType inputType);

	public override void _Input(InputEvent @event)
	{
		if(@event is InputEventJoypadMotion motionEvent)
		{
			if((motionEvent.Axis is JoyAxis.LeftX or JoyAxis.LeftY) && motionEvent.AxisValue < LeftJoystickDeadzone)
			{
				GetViewport().SetInputAsHandled();
				return;
			}
			else if((motionEvent.Axis is JoyAxis.RightX or JoyAxis.RightY) && motionEvent.AxisValue < RightJoystickDeadzone)
			{
				GetViewport().SetInputAsHandled();
				return;
			}
		}

		MostRecentInputType = @event switch
		{
			InputEventJoypadButton => InputType.Controller,
			InputEventJoypadMotion => InputType.Controller,
			_ => InputType.Keyboard
		};

		if(MostRecentInputType != _oldInputType)
		{
			_oldInputType = MostRecentInputType;

			EmitSignal(SignalName.InputTypeChanged, (int) MostRecentInputType);
		}
	}
}
