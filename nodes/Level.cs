using Godot;
using System.Linq;

namespace Epilogue.nodes;
[GlobalClass]
public partial class Level : Node2D
{
	public TileData GetTileDataAtPosition(Vector2 position)
	{
		var tileMap = GetChildren().OfType<TileMap>().FirstOrDefault();
		var localPosition = tileMap.LocalToMap(position);

		return tileMap.GetCellTileData(0, localPosition);
	}
}
