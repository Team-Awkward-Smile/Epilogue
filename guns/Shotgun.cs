using Epilogue.nodes;
using Godot;

namespace Epilogue.guns;
public partial class Shotgun : Gun
{
	[Export] public override int MaxAmmoCount { get; set; }
	[Export] public override float ShotsPerMinute { get; set; }

	private PackedScene _pelletScene;

	public override void _Ready()
	{
		base._Ready();

		_pelletScene = GD.Load<PackedScene>("res://temp/shotgun_pellet.tscn");
	}

	public override void OnTriggerPress()
	{
		var shotDelay = 1 / (ShotsPerMinute / 60);

		if(TimeSinceLastShot >= shotDelay)
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

			Events.EmitGlobalSignal("GunFired", CurrentAmmoCount);
		}
	}
}
