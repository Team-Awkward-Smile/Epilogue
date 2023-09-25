using Epilogue.global.enums;
using Epilogue.nodes;
using Godot;
using Godot.Collections;

namespace Epilogue.actors.rob.states;
/// <summary>
///		State that allows Rob to chase Hestmor around the map
/// </summary>
public partial class Chase : NpcState
{
	[Export] private float _movementSpeed = 80f;

	private Actor _bob;

	internal override void OnEnter(params object[] args)
	{
		_bob = GetNode<Actor>("../../../Bob");
		NavigationAgent.TargetPosition = _bob.Position;
		NavigationAgent.LinkReached += MoveToLink;

		AnimPlayer.Play("walk");
	}

	internal override void PhysicsUpdate(double delta)
	{
		NavigationAgent.TargetPosition = _bob.Position;

		if(NavigationAgent.IsNavigationFinished())
		{
			AnimPlayer.Play("idle");
			return;
		}

		AnimPlayer.Play("walk");

		var nextPosition = NavigationAgent.GetNextPathPosition();
		var newVelocity = (nextPosition - Npc.GlobalPosition).Normalized() * _movementSpeed;

		Npc.SetFacingDirection(newVelocity.X < 0 ? ActorFacingDirection.Left : ActorFacingDirection.Right);
		Npc.Velocity = new Vector2(newVelocity.X, Npc.Velocity.Y + Gravity * (float) delta);
		Npc.MoveAndSlideWithRotation();
	}

	private void MoveToLink(Dictionary details)
	{
		Npc.Position = (Vector2) details["link_exit_position"];
	}
}
