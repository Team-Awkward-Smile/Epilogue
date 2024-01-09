using Epilogue.Nodes;
using Epilogue.Global.Enums;
using Godot;

namespace Epilogue.Actors.MossPlant;
/// <summary>
///		Class the manages the projectiles shot by the Moss Plant of Guwama
/// </summary>
public partial class MossPlantProjectile : Projectile
{
	private RayCast2D _bounceRayCast2D;
	private bool _returning;
	private MossPlant _mossPlant;

	/// <inheritdoc/>
	public override void _Ready()
	{
		base._Ready();

		_bounceRayCast2D = GetNode<RayCast2D>("BounceRayCast2D");
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

		ValidAreaHit += (Area2D area) =>
		{
			if (area.Owner is Actor)
			{
				QueueFree();
			}
		};

		// Bounces around the map when hitting a surface
		ValidBodyHit += (Node2D body) =>
		{
			if (_returning)
			{
				return;
			}

			_bounceRayCast2D.ForceRaycastUpdate();

			var normal = _bounceRayCast2D.GetCollisionNormal();
			var rotationX = 180f * normal.X;
			var rotationY = normal.Y == 0f ? 1f : normal.Y;

			RotationDegrees = (RotationDegrees * Mathf.Abs(rotationY) * -1) + rotationX;
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
