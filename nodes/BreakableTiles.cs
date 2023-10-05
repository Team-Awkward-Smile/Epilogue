using Epilogue.global.enums;
using Godot;

namespace Epilogue.nodes;
/// <summary>
///		A special TileMap used for Tiles that can be destroyed by damage
/// </summary>
[GlobalClass]
public partial class BreakableTiles : TileMap
{
	/// <summary>
	///		HP of the Tile. Once it reaches 0, the Tile will be destroyed
	/// </summary>
	[Export] private float _health;

	/// <summary>
	///		Causes damage to this Tile. The actual damage caused may vary depending on the Damage Type
	/// </summary>
	/// <param name="damage">The base amount of damage to cause</param>
	/// <param name="damageType">The type of damage caused</param>
	public void DamageTile(float damage, DamageType damageType)
	{
		if(damageType == DamageType.Unarmed)
		{
			damage /= 2;
		}

		_health -= damage;

		if(_health <= 0)
		{
			DestroyTile();
		}
	}

	private void DestroyTile()
	{
		QueueFree();
	}
}
