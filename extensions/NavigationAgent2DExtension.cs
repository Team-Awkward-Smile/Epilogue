using Godot;

namespace Epilogue.extensions;
/// <summary>
/// 	Helper methods for NavigationAgent2D's
/// </summary>
public static class NavigationAgent2DExtension
{
	/// <summary>
	/// 	Gets the next velocity needed to reach the target destination
	/// </summary>
	/// <param name="navigationAgent">The NavigationAgent2D used for the path-finding of this Actor</param>
	/// <param name="actorGlobalPosition">The global position of the Actor that will be moved</param>
	/// <param name="actorMoveSpeed">The movement speed of the Actor that will be moved</param>
	/// <returns>A Vector2 with the X and Y speed the Actor needs to use during this frame to move towards it's destination</returns>
	public static Vector2 GetNextVelocity(this NavigationAgent2D navigationAgent, Vector2 actorGlobalPosition, float actorMoveSpeed)
	{
		return (navigationAgent.GetNextPathPosition() - actorGlobalPosition).Normalized() * actorMoveSpeed;
	}
}
