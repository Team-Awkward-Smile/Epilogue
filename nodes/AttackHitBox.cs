using Godot;
using System;

namespace Epilogue.nodes;
/// <summary>
///		Node for controlling the HitBoxes of attacks (usually melee, since projectiles have their own HitBoxes)
/// </summary>
[GlobalClass]
public partial class AttackHitBox : HitBox
{
	private CollisionShape2D _collisionShape;

	/// <summary>
	///		Spawns a new HitBox from the specified path
	/// </summary>
	/// <param name="resourcePath">The path of the Resource defining the HitBox</param>
	/// <param name="lifespan">Amount of time, in seconds, this HitBox will remain active before being destroyed</param>
	public void SpawnHitBox(string resourcePath, float lifespan)
	{
		_collisionShape = new CollisionShape2D()
		{
			Shape = GD.Load<Shape2D>(resourcePath)
		};

		AddChild(_collisionShape);

		GetTree().CreateTimer(lifespan).Timeout += () => _collisionShape.QueueFree();
	}
}
