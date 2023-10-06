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
	[Export] private float _damage;

	/// <summary>
	///		Type of damage caused by this HitBox
	/// </summary>
	[Export] private DamageType _damageType;

	/// <inheritdoc/>
	public override void _Ready()
	{
		AreaEntered += (Area2D area) =>
		{
			if(area.Owner is Actor actor)
			{
				actor.DealDamage(_damage);
			}
		};

		BodyEntered += (Node2D body) =>
		{
			if(body is BreakableTile tile)
			{
				tile.DamageTile(_damage, _damageType);
			}
		};
	}
}
