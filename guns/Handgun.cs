using Epilogue.nodes;
using Godot;

namespace Epilogue.guns;
public partial class Handgun : Gun
{
	[Export] public override int MaxAmmoCount { get; set; }
	[Export] public override float ShotsPerMinute { get; set; } 

	private float _shotDelay;
	private PackedScene _bulletScene;

	public override void _Ready()
	{
		base._Ready();

		_bulletScene = GD.Load<PackedScene>("res://temp/handgun_bullet.tscn");
	}

	public override void OnTriggerHeld(double delta)
	{
		if(CurrentAmmoCount > 0 && TimeSinceLastShot >= _shotDelay)
		{
			TimeSinceLastShot = 0;
			CurrentAmmoCount--;

			var bullet = _bulletScene.Instantiate() as Projectile;

			GetTree().Root.AddChild(bullet);

			bullet.GlobalTransform = Muzzle.GlobalTransform;

			AudioPlayer.Play();
		}
	}

	public override void OnTriggerPress()
	{
		_shotDelay = 1 / (ShotsPerMinute / 60);
	}
}
