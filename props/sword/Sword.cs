using Epilogue.nodes;
using Godot;

namespace Epilogue.props;
/// <summary>
///		Sword that can be acquired in a secret area in New Game +
/// </summary>
public partial class Sword : Gun
{
	private protected override void AfterReady()
	{
		ProjectileScene = GD.Load<PackedScene>("res://props/sword/sword_slash.tscn");
	}

	private protected override void OnTriggerPress()
	{
		if(TimeSinceLastShot >= ShotDelayPerSecond)
		{
			var slash = ProjectileScene.Instantiate() as Projectile;

			GetTree().Root.AddChild(slash);

			slash.GlobalTransform = Muzzle.GlobalTransform;

			TimeSinceLastShot = 0;
		}
	}
}
