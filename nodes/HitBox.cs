using System.Linq;
using Epilogue.global.enums;
using Epilogue.props.breakable_tile;
using Godot;

namespace Epilogue.nodes;
/// <summary>
///		Node used as the base for every HitBox in the game
/// </summary>
[GlobalClass, Tool]
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

    public override void _EnterTree()
    {
        if(GetChildCount() > 0)
        {
            GetChildren().OfType<CollisionShape2D>().First().DebugColor = new Color(1f, 70f / 255, 50f / 255, 107f / 255);
        }
    }

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
