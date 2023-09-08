using Godot;

namespace Epilogue.nodes;
[GlobalClass]
public partial class HitBox : Area2D
{
	/// <summary>
	///		Damage caused by this HitBox
	/// </summary>
	[Export] public float Damage { get; set; }

	public float BonusDamage { get; set; } = 0f;

    public Shape2D CollisionShape
	{
		get => _collisionShape;
		set
		{
			_collisionShape = value;
			SpawnCollisionShape();
		}
	}

	private Shape2D _collisionShape;

	private void SpawnCollisionShape()
	{
		AddChild(new CollisionShape2D()
		{
			Shape = CollisionShape,
		});
	}

	public void DeleteHitBox()
	{
		GetChild(0).QueueFree();
	}
}
