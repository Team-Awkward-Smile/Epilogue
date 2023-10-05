using Godot;
using System.Linq;

namespace Epilogue.nodes;
/// <summary>
///     Node used as the base for all projectiles
/// </summary>
[GlobalClass, Icon("res://nodes/icons/projectile.png")]
public partial class Projectile : HitBox
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
    ///     Type of damage caused by this projectile
    /// </summary>
    [Export] public int DamageType { get; set; }

    /// <summary>
    ///     Vertical force applied to this projectile while it travels. Positive values will pull the projectile down, and positive ones will make the projectile rise as it travels
    /// </summary>
    [Export] public float VerticalForce { get; set; } = 0f;

    /// <summary>
    ///     This projectile will be destroyed if it doesn't hit anything in this ammount of time
    /// </summary>
    [Export] public float LifeTime { get; set; }

    private float _timer;

	/// <inheritdoc/>
	public override void _Ready()
	{
        base._Ready();

		AreaEntered += (Area2D area) => 
		{
            if(DestroyOnHit)
            {
			    QueueFree();
            }
		};

        BodyEntered += (Node2D body) =>
        {
            if(DestroyOnHit)
            {
			    QueueFree();
            }
        };
	}

	/// <inheritdoc/>
	public override void _PhysicsProcess(double delta)
	{
        _timer += (float) delta;

        if(_timer >= LifeTime)
        {
            QueueFree();
        }

        Position += Transform.X * Speed * (float) delta;
	}
}
