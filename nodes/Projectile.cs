using Godot;

namespace Epilogue.Nodes;
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
	///     Speed of this projectile
	/// </summary>
	[Export] public Vector2 Speed { get; set; }

	[Export] private double _lifetime;

	private float _timer;

	/// <inheritdoc/>
	public override void _Ready()
	{
		base._Ready();

		AreaEntered += (Area2D area) =>
		{
			if (DestroyOnHit)
			{
				QueueFree();
			}
		};
	}

	/// <summary>
	///     Checks if the collision happened with an Actor. If so, deals damage to it
	/// </summary>
	/// <param name="area"></param>
	private void DamageActor(Area2D area)
	{
		if (area.Owner is Actor actor)
		{
			GD.Print($"Dealing [{Damage}] to [{actor.Name}]");
			actor.ReduceHealth(Damage, DamageType);
		}
	}

	/// <inheritdoc/>
	public override void _PhysicsProcess(double delta)
	{
		_timer += (float)delta;

		if (_timer >= _lifetime)
		{
			QueueFree();
		}

		Position += Speed.Rotated(Rotation) * (float)delta;
	}
}
