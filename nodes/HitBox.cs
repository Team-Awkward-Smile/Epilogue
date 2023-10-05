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
	///		Damage caused by this HitBox
	/// </summary>
	[Export] public float Damage { get; private set; }

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
				tile.DamageTile(Damage, DamageType.Piercing);
			}
		};
	}
}
