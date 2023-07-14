using Epilogue.global.enums;
using Epilogue.nodes;
using Godot;
using Godot.Collections;

namespace Epilogue.actors.rob.states;
public partial class Chase : NpcState
{
	[Export] private float _movementSpeed = 80f;

	private Actor _bob;

	public override void OnEnter()
	{
		_bob = GetNode<Actor>("../../../Bob");
		_navigationAgent.TargetPosition = _bob.Position;
		_navigationAgent.LinkReached += MoveToLink;

		AnimPlayer.Play("walk");
	}

	public override void PhysicsUpdate(double delta)
	{
		_navigationAgent.TargetPosition = _bob.Position;

		if(_navigationAgent.IsNavigationFinished())
		{
			AnimPlayer.Play("idle");
			return;
		}

		AnimPlayer.Play("walk");

		var nextPosition = _navigationAgent.GetNextPathPosition();
		var newVelocity = (nextPosition - Actor.GlobalPosition).Normalized() * _movementSpeed;

		Actor.SetFacingDirection(newVelocity.X < 0 ? ActorFacingDirectionEnum.Left : ActorFacingDirectionEnum.Right);
		Actor.Velocity = new Vector2(newVelocity.X, Actor.Velocity.Y + Gravity * (float) delta);
		Actor.MoveAndSlideWithRotation();
	}

	private void MoveToLink(Dictionary details)
	{
		Actor.Position = (Vector2) details["link_exit_position"];
	}
}
