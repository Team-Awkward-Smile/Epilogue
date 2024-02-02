using Epilogue.Nodes;
using Epilogue.Global.Enums;
using Godot;
using Epilogue.Extensions;

namespace Epilogue.Actors.MossPlant;
/// <summary>
///		Class the manages the projectiles shot by the Moss Plant of Guwama
/// </summary>
public partial class MossPlantProjectile : Projectile
{
	private ShapeCast2D _bounceShapeCast2D;
	private bool _returning;
	private MossPlant _mossPlant;
	private ulong _collisionFrame;

	/// <inheritdoc/>
	public override void _Ready()
	{
		base._Ready();

		_bounceShapeCast2D = GetNode<ShapeCast2D>("BounceShapeCast2D");
		_mossPlant = (MossPlant)Owner;

		// Checks if the Area hit was a Melee Attack or the Plant that shot this projectile
		AreaEntered += (Area2D area) =>
		{
			if (area is HitBox hitBox && hitBox.DamageType == DamageType.Unarmed)
			{
				ReturnToOwner();
			}
			else if (_returning && area.Owner == _mossPlant)
			{
				_mossPlant.OnProjectileReturned();

				QueueFree();
			}
		};

		ActorHit += QueueFree;

		// Bounces around the map when hitting a surface
		BodyEntered += (Node2D body) =>
		{
			var currentFrame = Engine.GetPhysicsFrames();

			if (_returning || _collisionFrame == currentFrame)
			{
				return;
			}

			_collisionFrame = currentFrame;

			_bounceShapeCast2D.ForceShapecastUpdate();

			var normal = _bounceShapeCast2D.GetCollisionNormal(0);
			var bounceVector = Speed.Rotated(Rotation).Bounce(normal);

			Rotation = bounceVector.Angle();
		};

		TreeExiting += () => _mossPlant.IsProjectileActive = false;
	}

	private void ReturnToOwner()
	{
		var targetPosition = _mossPlant.GlobalPosition - GlobalPosition;

		Speed *= 2f;
		Rotation = targetPosition.Angle();

		_returning = true;
	}
}
