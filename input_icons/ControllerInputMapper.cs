using Epilogue.Global.Enums;

using Godot;

namespace Epilogue.InputIcons;
/// <summary>
///		Class that automatically maps an InputEvent coming from a controller to it's icon
/// </summary>
public class ControllerInputMapper
{
	/// <summary>
	///		Maps the InputEvent originating from a controller to it's correct icon, depending on the brand of controller selected by the player
	/// </summary>
	/// <param name="brand">Brand of the controller, used to determine which icon set to use</param>
	/// <param name="event">The event that will be mapped</param>
	/// <returns>A CompressedTexture2D of the correct icon</returns>
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
