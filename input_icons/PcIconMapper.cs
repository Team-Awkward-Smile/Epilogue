using Godot;

namespace Epilogue.InputIcons;
/// <summary>
///		Class that automatically maps mouse button events to their corresponding icons
/// </summary>
public class PcIconMapper
{
	/// <summary>
	///		Maps the event originating from a mouse click to it's corresponding icon
	/// </summary>
	/// <param name="event">The event to be mapped</param>
	/// <returns>A CompressedTexture2D of the corresponding icon</returns>
	public static CompressedTexture2D GetIconForEvent(InputEvent @event)
	{
		// TODO: 36 - This class needs more work
		var basePath = "res://input_icons/pc/";

		if(@event is InputEventKey key)
		{
			return GD.Load<CompressedTexture2D>($"{basePath}{key.PhysicalKeycode}-Key.png");
		}

		return null;
	}
}
