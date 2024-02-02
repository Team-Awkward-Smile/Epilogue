using Epilogue.Nodes;
using Godot;

namespace Epilogue.Guns;
/// <summary>
///		Minigun for testing purposes
/// </summary>
public partial class Minigun : Gun
{
	private PackedScene _bulletScene;
	private float _timeShooting;
	private float _baseShotsPerMinute;

	private protected override void AfterReady()
	{
		_bulletScene = GD.Load<PackedScene>("res://temp/minigun_bullet.tscn");
		_baseShotsPerMinute = ShotsPerMinute;
	}

	private protected override void OnTriggerHeld(double delta)
	{
		_timeShooting += (float) delta;

		ShotsPerMinute += _timeShooting * 5;

		if(CurrentAmmoCount > 0 && TimeSinceLastShot >= 1 / (ShotsPerMinute / 60))
		{
			TimeSinceLastShot = 0f;

			var bullet = _bulletScene.Instantiate() as Projectile;

			GetTree().Root.AddChild(bullet);

			bullet.GlobalTransform = Muzzle.GlobalTransform;

			var angle = Mathf.Min(ShotsPerMinute / 70, 15f);

			bullet.RotationDegrees += new RandomNumberGenerator().RandfRange(-angle, angle);

			CurrentAmmoCount--;

			GunEvents.EmitGlobalSignal("GunFired", CurrentAmmoCount);
		}
	}

	private protected override void OnTriggerRelease()
	{
		_timeShooting = 0f;
		ShotsPerMinute = _baseShotsPerMinute;
	}
}
