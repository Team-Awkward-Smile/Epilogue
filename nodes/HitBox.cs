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

	public float BonusDamage { get; set; } = 0f;

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

	public void DeleteHitBox()
	{
		GetChild(0).QueueFree();
	}

	/// <inheritdoc/>
	public override void _Ready()
	{
		AreaEntered += (Area2D area) =>
		{
			GD.Print("hit");
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
