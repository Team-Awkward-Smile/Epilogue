using Godot;
using System;

namespace Epilogue.nodes;
/// <summary>
///		Node for controlling the HitBoxes of attacks (usually melee, since projectiles have their own HitBoxes)
/// </summary>
[GlobalClass, Tool]
public partial class AttackHitBox : Area2D
{
	[Export] private float HitBoxDamage { get; set; }

	private CollisionShape2D _collisionShape;

	public override void _EnterTree()
	{
		CollisionLayer = 1 << 4;
		CollisionMask = 1 << 5;

		if(!Engine.IsEditorHint())
		{
			AreaEntered += DealDamage;
		}
	}

	public void SpawnHitBox(string resourcePath)
	{
		_collisionShape = new CollisionShape2D()
		{ 
			Shape = GD.Load<Shape2D>(resourcePath)
		};

		AddChild(_collisionShape);
	}

	public void DestroyHitBox()
	{
		_collisionShape.QueueFree();
	}

	private void DealDamage(Node2D area)
	{
		if(area.Owner is Actor enemy)
		{
			enemy.Health.DealDamage(HitBoxDamage);
		}
	}
}
