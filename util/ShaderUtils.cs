using Godot;

namespace Epilogue.util;
/// <summary>
///		Methods that are useful when dealing with Shaders
/// </summary>
public static class ShaderUtils
{
	/// <summary>
	///		Gets the position of the informed CanvasItem, translated to the screen's UV.
	///		The UV ranges from (0, 0) in the top-left, to (1, 1) in the bottom-right
	/// </summary>
	/// <returns>A Vector2 representing that position in the screen UV. Values lower than 0 or greater than 1 means the object is not currently visible</returns>
	/// <example>A result of (0.5, 0.5) means the object is in the center of the screen, regardless of it's actual position in the world space</example>
	public static Vector2 GetCanvasItemPositionInScreenUV(CanvasItem canvasItem)
	{
		var o = canvasItem.GetGlobalTransformWithCanvas().Origin;
		var sz = DisplayServer.WindowGetSize();

		return new Vector2(o.X / sz.X, (o.Y / sz.Y));
	}
}
