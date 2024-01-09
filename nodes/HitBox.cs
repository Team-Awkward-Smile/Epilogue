using Epilogue.Global.Enums;
using Epilogue.Props.BreakableTile;
using Godot;

namespace Epilogue.Nodes;
/// <summary>
///		Node used as the base for every HitBox in the game
/// </summary>
[GlobalClass]
public partial class HitBox : Area2D
{
	/// <summary>
	///		Signal emitted whenever a valid Area2D is hit (does not register hits against the Owner of the HitBox)
	/// </summary>
	/// <param name="area">The area that was hit</param>
	[Signal] public delegate void ValidAreaHitEventHandler(Area2D area);

	/// <summary>
	///		Signal emitted whenever a valid Node2D is hit
	/// </summary>
	/// <param name="node">The node that was hit</param>
	[Signal] public delegate void ValidBodyHitEventHandler(Node2D node);

	/// <summary>
	///		Type of damage caused by this HitBox
	/// </summary>
	[Export] public DamageType DamageType { get; private set; }

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
		Owner ??= GetParent();

		AreaEntered += (Area2D area) =>
		{
			// Ignores the parent Node2D, never damaging it
			if (area is HurtBox && area.Owner != Owner && area.Owner is Actor actor)
			{
				EmitSignal(SignalName.ValidAreaHit, area);

				actor.ReduceHealth(Damage, DamageType);
			}
		};

		BodyEntered += (Node2D body) =>
		{
			EmitSignal(SignalName.ValidBodyHit, body);

			if (body is BreakableTile tile)
			{
				tile.DamageTile(Damage, DamageType);
			}
		};
	}
}
