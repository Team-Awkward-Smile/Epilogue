using Epilogue.nodes;
using Godot;

namespace Epilogue.actors.hestmor.states;
/// <summary>
///		State that allows Hestmor to vault over small obstacles
/// </summary>
public partial class Vault : PlayerState
{
	private Vector2 _spriteOriginalPosition;

	internal override void OnEnter(params object[] args)
	{
		_spriteOriginalPosition = Player.Sprite.Position;

		Player.CanChangeFacingDirection = false;
		Player.Velocity = Vector2.Zero;

		AnimPlayer.Play("vault");
		AnimPlayer.AnimationFinished += MoveToTop;
	}

	private void MoveToTop(StringName animName)
	{
		AnimPlayer.AnimationFinished -= MoveToTop;

		Player.GlobalPosition = Player.Sprite.GetNode<Node2D>("LedgeAnchor").GlobalPosition;
		Player.Sprite.Position = _spriteOriginalPosition;

		StateMachine.ChangeState("Fall");
	}
}

