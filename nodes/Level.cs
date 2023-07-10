using Godot;
using System.Linq;

namespace Epilogue.nodes;
/// <summary>
///		Base class for all Levels (GUIs are not Levels)
/// </summary>
[GlobalClass]
public partial class Level : Node2D
{
	/// <summary>
	///		Gets the TileData of the tile at the informed position
	/// </summary>
	/// <param name="position">Desired position, in global coordinates</param>
	/// <returns>If there's a cell at the informed position, the <see cref="TileData"/> of that cell; otherwise, <c>null</c></returns>
	public TileData GetTileDataAtPosition(Vector2 position)
	{
		var tileMap = GetChildren().OfType<TileMap>().FirstOrDefault();
		var localPosition = tileMap.LocalToMap(position);

		return tileMap.GetCellTileData(0, localPosition);
	}
}
