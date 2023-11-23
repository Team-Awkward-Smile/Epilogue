using Epilogue.global.enums;
using Epilogue.props.breakable_tile;
using Godot;

namespace Epilogue.nodes;
/// <summary>
///		Node used as the base for every HitBox in the game
/// </summary>
[GlobalClass]
public partial class HitBox : Area2D
{
	/// <summary>
	///		Type of damage caused by this HitBox
	/// </summary>
	[Export] private DamageType _damageType;

	/// <summary>
	///		Damage caused by this HitBox
	/// </summary>
	[Export] public float Damage { get; set; }

	/// <summary>
	/// 	Bonus damage (if any) caused by this HitBox on a hit
	/// </summary>
	public float BonusDamage { get; set; } = 0f;

	/// <summary>
	/// 	The CollisionShape used by this HitBox to detect collisions
	/// </summary>
    public Shape2D CollisionShape
	{
		get => _collisionShape;
		set
		{
			_collisionShape = value;
			SpawnCollisionShape();
		}
	}

	private Shape2D _collisionShape;

	private void SpawnCollisionShape()
	{
		AddChild(new CollisionShape2D()
		{
			Shape = CollisionShape,
		});
	}

	/// <summary>
	/// 	Deletes a previously created HitBox
	/// </summary>
	public void DeleteHitBox()
	{
		GetChild(0).QueueFree();
	}

	/// <inheritdoc/>
	public override void _Ready()
	{
		AreaEntered += (Area2D area) =>
		{
			if(area.Owner is Actor actor)
			{
				actor.DealDamage(Damage);
			}
		};

		BodyEntered += (Node2D body) =>
		{
			if(body is BreakableTile tile)
			{
				tile.DamageTile(Damage, _damageType);
			}
		};
	}
}
