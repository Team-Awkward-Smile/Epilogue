using Epilogue.nodes;

using Godot;

namespace Epilogue.guns;
/// <summary>
///		Flamethrower used for testing purposes
/// </summary>
public partial class Flamethrower : Gun
{
	private PackedScene _flameScene;
	private float _shotDelay;
	private PackedScene _streamScene;
	private FlameStream _flameStream;

	private protected override void AfterReady()
	{
		_flameScene = GD.Load<PackedScene>("res://temp/flame.tscn");
		_streamScene = GD.Load<PackedScene>("res://temp/flame_stream.tscn");
	}

	private protected override void OnTriggerHeld(double delta)
	{
		if(TimeSinceLastShot >= _shotDelay)
		{
			TimeSinceLastShot = 0;
			CurrentAmmoCount--;

			var flame = _flameScene.Instantiate() as Projectile;

			GetTree().Root.AddChild(flame);

			flame.GlobalTransform = Muzzle.GlobalTransform;

			GunEvents.EmitGlobalSignal("GunFired", CurrentAmmoCount);

			_flameStream.AddProjectile(flame);
		}
	}

	private protected override void OnTriggerPress()
	{
		_shotDelay = 1 / (ShotsPerMinute / 60);

		_flameStream = _streamScene.Instantiate() as FlameStream;

		AddChild(_flameStream);
	}
}
