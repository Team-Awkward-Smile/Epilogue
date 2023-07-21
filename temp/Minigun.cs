using Epilogue.nodes;
using Godot;
using System;

public partial class Minigun : Gun
{
	private PackedScene _bulletScene;
	private float _timeShooting;
	private float _baseShotsPerMinute;

	public override void _Ready()
	{
		base._Ready();

		_bulletScene = GD.Load<PackedScene>("res://temp/minigun_bullet.tscn");
		_baseShotsPerMinute = ShotsPerMinute;
	}

	public override void OnTriggerHeld(double delta)
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

			Events.EmitGlobalSignal("GunFired", CurrentAmmoCount);
		}
	}

	public override void OnTriggerRelease()
	{
		_timeShooting = 0f;
		ShotsPerMinute = _baseShotsPerMinute;
	}
}
