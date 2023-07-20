using Epilogue.nodes;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Epilogue.guns;
public partial class Flamethrower : Gun
{
	private PackedScene _flameScene;
	private float _shotDelay;
	private PackedScene _streamScene;
	private FlameStream _flameStream;

	public override void _Ready()
	{
		base._Ready();

		_flameScene = GD.Load<PackedScene>("res://temp/flame.tscn");
		_streamScene = GD.Load<PackedScene>("res://temp/flame_stream.tscn");
	}

	public override void OnTriggerHeld(double delta)
	{
		if(TimeSinceLastShot >= _shotDelay)
		{
			TimeSinceLastShot = 0;
			CurrentAmmoCount--;

			var flame = _flameScene.Instantiate() as Projectile;

			GetTree().Root.AddChild(flame);

			flame.GlobalTransform = Muzzle.GlobalTransform;

			Events.EmitGlobalSignal("GunFired", CurrentAmmoCount);

			_flameStream.AddProjectile(flame);
		}
	}

	public override void OnTriggerPress()
	{
		_shotDelay = 1 / (ShotsPerMinute / 60);

		_flameStream = _streamScene.Instantiate() as FlameStream;

		AddChild(_flameStream);
	}
}
