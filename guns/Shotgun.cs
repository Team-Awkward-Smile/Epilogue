using Epilogue.Nodes;
using Godot;

namespace Epilogue.Guns;
/// <summary>
///		Shotgun used for testing purposes
/// </summary>
public partial class Shotgun : Gun
{
	private PackedScene _pelletScene;

	private protected override void AfterReady()
	{
		_pelletScene = GD.Load<PackedScene>("res://temp/shotgun_pellet.tscn");
	}

	private protected override void OnTriggerPress()
	{
		if(TimeSinceLastShot >= ShotDelayPerSecond)
		{
			AudioPlayer.Play();

			TimeSinceLastShot = 0;
			CurrentAmmoCount--;

			var rand = new RandomNumberGenerator();

			for(var i = 0; i < 5; i++)
			{
				var pellet = _pelletScene.Instantiate() as Projectile;

				GetTree().Root.AddChild(pellet);

				pellet.GlobalTransform = Muzzle.GlobalTransform;
				pellet.RotationDegrees += rand.RandfRange(-10f, 10f);
			}

			GunEvents.EmitGlobalSignal("GunFired", CurrentAmmoCount);
		}
	}
}
