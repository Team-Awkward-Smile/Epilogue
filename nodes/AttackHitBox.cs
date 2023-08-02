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

	/// <inheritdoc/>
	public override void _EnterTree()
	{
		CollisionLayer = 1 << 4;
		CollisionMask = 1 << 5;

		if(!Engine.IsEditorHint())
		{
			AreaEntered += DealDamage;
		}
	}

	/// <summary>
	///		Spawns a new HitBox from the specified path
	/// </summary>
	/// <param name="resourcePath">The path of the Resource defining the HitBox</param>
	public void SpawnHitBox(string resourcePath)
	{
		_collisionShape = new CollisionShape2D()
		{ 
			Shape = GD.Load<Shape2D>(resourcePath)
		};

		AddChild(_collisionShape);
	}

	/// <summary>
	///		Destroys the current HitBox
	/// </summary>
	public void DestroyHitBox() => _collisionShape.QueueFree();

	private void DealDamage(Node2D area)
	{
		if(area.Owner is Npc enemy)
		{
			enemy.Health.DealDamage(HitBoxDamage);
		}
	}
}
