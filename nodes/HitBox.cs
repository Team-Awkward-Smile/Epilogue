using Epilogue.global.enums;
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
			switch(area.Owner)
			{
				case Actor actor:
					actor.DealDamage(Damage);
					break;

				case BreakableTiles tile:
					tile.DamageTile(Damage, DamageType.Piercing);
					break;
			}
		};
	}
}
