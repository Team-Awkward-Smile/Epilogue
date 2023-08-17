using Epilogue.global.enums;

using Godot;

namespace Epilogue.input_icons;
public class ControllerInputMapper
{
	public static CompressedTexture2D GetIconForEvent(InputDeviceBrand brand, InputEvent @event)
	{
		string path = null;
		var basePath = $"res://input_icons/{brand.ToString().ToLower()}/";

		if(@event is InputEventJoypadButton button)
		{
			path = $"{basePath}{button.ButtonIndex}.png";

		}
		else if(@event is InputEventJoypadMotion analog)
		{
			var texture = analog.Axis.ToString() + (analog.AxisValue < 0f ? "n" : "p");

			path = $"{basePath}{texture}.png";
		}

		if(path is not null)
		{
			return GD.Load<CompressedTexture2D>(path);
		}

		return null;
	}
}
