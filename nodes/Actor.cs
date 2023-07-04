using Epilogue.global.enums;
using Godot;
using System.Linq;

namespace Epilogue.nodes;
[GlobalClass]
public partial class Actor : CharacterBody2D
{
	public ActorFacingDirection FacingDirection { get; private set; } = ActorFacingDirection.Right;

	/// <summary>
	///		Checks if the RayCast2D of the character's head is colliding against anything
	/// </summary>
	public bool IsHeadRayCastColliding()
	{
		return GetNode<RayCast2D>("RotationContainer/HeadRayCast2D").IsColliding();
	}

	/// <summary>
	///		Checks if the RayCast2D of the character's waist is colliding against anything
	/// </summary>
	public bool IsWaistRayCastColliding()
	{
		return GetNode<RayCast2D>("RotationContainer/WaistRayCast2D").IsColliding();
	}

	/// <summary>
	///		Gets the hitbox used for the collisions of the character's body
	/// </summary>
	public CollisionShape2D GetMainHitbox()
	{
		var hitboxNode = (CollisionShape2D) GetChildren().Where(c => c.IsInGroup("MainHitbox")).FirstOrDefault();
		return hitboxNode;
	}

	/// <summary>
	///		Return a Vector2 representing the size of the Main Hitbox.
	///		The returned Vector's X and Y represent different things, depending on the type of hitbox:
	///		<list type="bullet">
	///			<item>RectangleShape2D: <c>Vector2(Size.X, Size.Y)</c></item>
	///		</list>
	/// </summary>
	/// <returns>A Vector2 with the size of the hitbox. The meaning of X and Y depends on the type of hitbox</returns>
	public Vector2 GetMainHitboxSize()
	{
		var hitbox = GetMainHitbox().Shape;
		var size = new Vector2();

		if(hitbox is RectangleShape2D)
		{
			size = (Vector2) hitbox.Get("size");
		}

		return size;
	}

	public void SetFacingDirection(ActorFacingDirection newDirection)
	{
		var rotationContainer = GetNode<Node2D>("RotationContainer");
		var scaleX = newDirection switch
		{
			ActorFacingDirection.Left => -1,
			ActorFacingDirection.Right => 1,
			_ => 1
		};

		FacingDirection = newDirection;

		rotationContainer.Scale = new Vector2(scaleX, 1f);
	}
}
