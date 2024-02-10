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
	///		Signal emitted whenever this HitBox hits an Actor (in other words, it collides with a HurtBox that is a child of an Actor)
	/// </summary>
	[Signal] public delegate void ActorHitEventHandler();

	/// <summary>
	///		Signal emitted whenever a tile is hit by this HitBox
	/// </summary>
	/// <param name="damageType">Type of the damage caused by the HitBox</param>
	/// <param name="isTileBreakable">Defines if the tile hit is breakable or not</param>

	[Signal] public delegate void TileHitEventHandler(DamageType damageType, bool isTileBreakable);

	/// <summary>
	///		Type of damage caused by this HitBox
	/// </summary>
	[Export] public DamageType DamageType { get; set; }

	/// <summary>
	///		Damage caused by this HitBox
	/// </summary>
	[Export] public float Damage { get; set; }

	/// <summary>
	/// 	Bonus damage (if any) caused by this HitBox on a hit
	/// </summary>
	public float BonusDamage { get; set; } = 0f;

	/// <inheritdoc/>
	public override void _Ready()
	{
		Owner ??= GetParent();

		AreaEntered += (Area2D area) =>
		{
			if (area.Owner != Owner && area is HurtBox hurtBox)
			{
				EmitSignal(SignalName.ActorHit);

				hurtBox.OnHit(this);
			}
		};

		BodyEntered += (Node2D body) =>
		{
			if (body is BreakableTile tile)
			{
				tile.DamageTile(Damage, DamageType);
			}
		};
	}
}
