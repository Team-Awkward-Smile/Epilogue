using System;

namespace Epilogue.global.enums;
/// <summary>
///		Layers used by the collision system, defined as an enum to avoid having to use numbers everywhere
/// </summary>
[Flags]
public enum CollisionLayerName
{
	/// <summary>
	///		Empty value used to clean the layers assigned to a Node
	/// </summary>
	None = 0,

	/// <summary>
	///		Layer dedicated to the Player, containing their CharacterBody2D
	/// </summary>
	Player = 1,

	/// <summary>
	///		Layer containing the CharacterBody2D of enemies
	/// </summary>
	Enemies = 1 << 1,

	/// <summary>
	///		Layer containing the colliders of objects scattered around the world
	/// </summary>
	Props = 1 << 2,

	/// <summary>
	///		Layer containing the TileMap of the Level, alongside it's collisions
	/// </summary>
	World = 1 << 3,

	/// <summary>
	///		Layer containing Area2D's capable of dealing damage to HurtBoxes
	/// </summary>
	HitBoxes = 1 << 4,

	/// <summary>
	///		Layer containing Area2D's capable of reacting to damage caused bu HitBoxes
	/// </summary>
	HurtBoxes = 1 << 5,

	/// <summary>
	///		Layer containing Area2D's of objects the player can interact with (guns, swords, doors, etc.)
	/// </summary>
	Interactives = 1 << 6
}