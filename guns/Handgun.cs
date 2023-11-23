using Epilogue.nodes;
using Godot;

namespace Epilogue.guns;
/// <summary>
///		Handgun used for testing purposes
/// </summary>
public partial class Handgun : Gun
{
	private PackedScene _bulletScene;

    private protected override void AfterReady()
	{
		_bulletScene = GD.Load<PackedScene>("res://temp/handgun_bullet.tscn");
	}

	private protected override void OnTriggerHeld(double delta)
	{
		if(CurrentAmmoCount > 0 && TimeSinceLastShot >= ShotDelayPerSecond)
		{
			TimeSinceLastShot = 0;
			CurrentAmmoCount--;

			var bullet = _bulletScene.Instantiate() as Projectile;

			GetTree().Root.AddChild(bullet);

			bullet.GlobalTransform = Muzzle.GlobalTransform;

			AudioPlayer.Play();
			GunEvents.EmitGlobalSignal("GunFired", CurrentAmmoCount);
		}
	}
}
