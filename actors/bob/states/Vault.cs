using Epilogue.nodes;
using Godot;

namespace Epilogue.actors.hestmor.states;
/// <summary>
///		State that allows Hestmor to vault over small obstacles
/// </summary>
public partial class Vault : PlayerState
{
	internal override void OnEnter()
	{
		Player.CanChangeFacingDirection = false;

		AnimPlayer.Play("ledge_climb");
		AnimPlayer.AnimationFinished += MoveToTop;
	}

	private void MoveToTop(StringName animName)
	{
		AnimPlayer.AnimationFinished -= MoveToTop;

		Player.GlobalPosition = Player.Sprite.GetNode<Node2D>("LedgeAnchor").GlobalPosition;

		StateMachine.ChangeState("Fall");
	}
}

