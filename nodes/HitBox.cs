using Epilogue.Global.Enums;
using Epilogue.props.breakable_tile;
using Godot;

namespace Epilogue.Nodes;
/// <summary>
///		Node used as the base for every HitBox in the game
/// </summary>
[GlobalClass]
public partial class HitBox : Area2D
{
	[Signal] public delegate void ActorHitEventHandler(Actor actor);

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
		AreaEntered += (Area2D area) =>
		{
			if (area.Owner is Actor actor)
			{
				EmitSignal(SignalName.ActorHit, actor);
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
