using Godot;
using System;

public static class NavigationAgent2DExtension
{
	public static Vector2 GetNextVelocity(this NavigationAgent2D navigationAgent, Vector2 actorGlobalPosition, float actorMoveSpeed)
	{
		return (navigationAgent.GetNextPathPosition() - actorGlobalPosition).Normalized() * actorMoveSpeed;
	}
}
