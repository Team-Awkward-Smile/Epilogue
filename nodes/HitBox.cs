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
	[Signal] public delegate void ActorHitEventHandler();

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
			if (area.Owner is Actor actor)
			{
				EmitSignal(SignalName.ActorHit);
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
