using Epilogue.nodes;

using Godot;
using System;

namespace Epilogue.actors.rob.states;
public partial class Shoot : NpcState
{
	internal override void OnEnter(params object[] args)
	{
		Npc.TurnTowards(Player);

		AnimPlayer.PlayBackwards("Combat/shoot");
		AnimPlayer.AnimationFinished += OnAnimationFinish;
	}

	private void OnAnimationFinish(StringName animName)
	{
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
