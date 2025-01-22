using Godot;

namespace Epilogue.Global.DTO;
public class SpriteSheetDataDto
{
	public int ID { get; set; }
	public CompressedTexture2D Texture { get; set; }
	public int HFrames { get; set; }
	public int VFrames { get; set; }
	public Vector2 Scale { get; set; }
}
