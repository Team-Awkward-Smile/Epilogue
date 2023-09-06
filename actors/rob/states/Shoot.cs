using Epilogue.nodes;

using Godot;
using System;

namespace Epilogue.actors.rob.states;
public partial class Shoot : NpcState
{
	internal override void OnEnter()
	{
		Npc.CanChangeFacingDirection = false;

		Npc.Sprite.Frame = 0;
		AnimPlayer.PlayBackwards("Combat/shoot");
		AnimPlayer.AnimationFinished += OnAnimationFinish;
	}

	private void OnAnimationFinish(StringName animName)
	{
		Npc.CanChangeFacingDirection = true;

		AnimPlayer.AnimationFinished -= OnAnimationFinish;
		StateMachine.ChangeState("Move");
	}

	public void SpawnProjectile()
	{
		var _projectile = GD.Load<PackedScene>("res://actors/rob/projectiles/projectile.tscn").Instantiate() as Projectile;
		GetTree().Root.AddChild(_projectile);

		_projectile.GlobalTransform = GetNode<Node2D>("../../FlipRoot/ProjectileSpawn").GlobalTransform;
	}
}
