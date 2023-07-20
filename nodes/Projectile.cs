using Godot;
using System.Linq;

namespace Epilogue.nodes;
/// <summary>
///     Node used as the base for all projectiles
/// </summary>
[GlobalClass]
public partial class Projectile : Area2D
{
    /// <summary>
    ///     Should this projectile be destroyed when colliding against something?
    /// </summary>
    [Export] public bool DestroyOnHit { get; set; } = true;

    /// <summary>
    ///     Horizontal speed of this projectile
    /// </summary>
    [Export] public float Speed { get; set; }

    /// <summary>
    ///     Damage caused by this projectile when colliding against an object that can be damaged
    /// </summary>
    [Export] public float Damage { get; set; }

    /// <summary>
    ///     Type of damage caused by this projectile
    /// </summary>
    [Export] public int DamageType { get; set; }

    /// <summary>
    ///     Vertical force applied to this projectile while it travels. Positive values will pull the projectile down, and positive ones will make the projectile rise as it travels
    /// </summary>
    [Export] public float VerticalForce { get; set; } = 0f;

	public override void _Ready()
	{
		if(DestroyOnHit)
        {
            AreaEntered += (Area2D area) => 
            {
                DamageActor(area);
                QueueFree();
            };
            BodyEntered += (Node2D body) =>
            {
                DamageWorld();
                QueueFree();
            };
        }
	}

    private void DamageActor(Area2D area)
    {
        if(area.Owner is Actor enemy && enemy.Health is NPCHealth npcHealth)
        {
		    npcHealth.DealDamage(Damage);
        }
    }

    private void DamageWorld()
    {
        GetTree().Root.GetChildren().OfType<Level>().FirstOrDefault().DamageTile(GlobalPosition, Damage);
    }

	public override void _PhysicsProcess(double delta)
	{
        Position += Transform.X * Speed * (float) delta;
	}
}
