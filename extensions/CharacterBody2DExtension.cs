using Godot;
using System.Linq;

namespace Epilogue.extensions
{
	public static class CharacterBody2DExtension
	{
		public static Node2D GetRotationContainer(this CharacterBody2D character)
		{
			return character.GetNode<Node2D>("RotationContainer");
		}

		/// <summary>
		///		Checks if the RayCast2D of the character's head is colliding against anything
		/// </summary>
		public static bool IsHeadRayCastColliding(this CharacterBody2D character)
		{
			return character.GetNode<RayCast2D>("RotationContainer/HeadRayCast2D").IsColliding();
		}

		/// <summary>
		///		Checks if the RayCast2D of the character's waist is colliding against anything
		/// </summary>
		public static bool IsWaistRayCastColliding(this CharacterBody2D character)
		{
			return character.GetNode<RayCast2D>("RotationContainer/WaistRayCast2D").IsColliding();
		}

		/// <summary>
		///		Gets the hitbox used for the collisions of the character's body
		/// </summary>
		public static CollisionShape2D GetMainHitbox(this CharacterBody2D character)
		{
			var hitboxNode = (CollisionShape2D) character.GetChildren().Where(c => c.IsInGroup("MainHitbox")).FirstOrDefault();
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
		public static Vector2 GetMainHitboxSize(this CharacterBody2D character)
		{
			var hitbox = character.GetMainHitbox().Shape;
			var size = new Vector2();

			if(hitbox is RectangleShape2D)
			{
				size = (Vector2) hitbox.Get("size");
			}

			return size;
		}
	}
}
