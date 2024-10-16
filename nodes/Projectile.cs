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

		ActorHit += () =>
		{
			if (DestroyOnHit)
			{
				QueueFree();
			}
		};
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
