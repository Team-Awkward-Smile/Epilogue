using Epilogue.global.enums;

using Godot;

namespace Epilogue.input_icons;
public class PcIconMapper 
{
	public static CompressedTexture2D GetIconForEvent(InputEvent @event)
	{
		var basePath = "res://input_icons/pc/";

		if(@event is InputEventKey key)
		{
			return GD.Load<CompressedTexture2D>($"{basePath}{key.PhysicalKeycode}-Key.png");
		}

		return null;
	}
}
