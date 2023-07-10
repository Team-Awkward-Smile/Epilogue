using Epilogue.nodes;
using Godot;

namespace Epilogue.actors.hestmor.states;
public partial class Vault : PlayerState
{
	public override void OnEnter()
	{
		Actor.CanChangeFacingDirection = false;

		AnimPlayer.Play("ledge_climb");
		AnimPlayer.AnimationFinished += MoveToTop;
	}

	private void MoveToTop(StringName animName)
	{
		AnimPlayer.AnimationFinished -= MoveToTop;

		Actor.GlobalPosition = Actor.Sprite.GetNode<Node2D>("LedgeAnchor").GlobalPosition;

		StateMachine.ChangeState("Fall");
	}
}

