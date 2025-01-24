using Epilogue.Global.Enums;
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

	[Export] private bool _notifyTarget = true;

	private float _timer;

	/// <inheritdoc/>
	public override void _Ready()
	{
		base._Ready();

		if (_notifyTarget)
		{
			var rayCast = new RayCast2D()
			{
				Enabled = true,
				TargetPosition = new Vector2(Speed.X * (float)_lifetime, 0f),
				CollisionMask = (uint)(CollisionLayerName.World | CollisionLayerName.Enemies)
			};

			AddChild(rayCast);

            rayCast.GlobalTransform = GlobalTransform;

			rayCast.ForceRaycastUpdate();

			if (rayCast.IsColliding() && rayCast.GetCollider() is Npc enemy)
			{
				enemy.TriggerProjectileNotification();
			}

			rayCast.QueueFree();
		}

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
