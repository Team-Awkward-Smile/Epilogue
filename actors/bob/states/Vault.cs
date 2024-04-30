using Epilogue.Nodes;
using Godot;

namespace Epilogue.Actors.Hestmor.States;
/// <inheritdoc/>
public partial class Vault : State
{
	private readonly Player _player;

	private Vector2 _spriteOriginalPosition;

	/// <summary>
	/// 	State that allows Hestmor to vault over small obstacles
	/// </summary>
	/// <param name="stateMachine">The State Machine who owns this State</param>
	public Vault(StateMachine stateMachine) : base(stateMachine)
	{
		_player = (Player)stateMachine.Owner;
	}

	internal override void OnStateMachineActivation()
	{
		AnimPlayer.AnimationFinished += (StringName animationName) =>
		{
			if (!Active || animationName != "vault")
			{
				return;
			}

			MoveToTop();
		};
	}

	internal override void OnEnter(params object[] args)
	{
		_spriteOriginalPosition = _player.Sprite.Position;

		_player.CanChangeFacingDirection = false;
		_player.Velocity = Vector2.Zero;

		AnimPlayer.Play("vault");
	}

	private void MoveToTop()
	{
		_player.GlobalPosition = _player.Sprite.GetNode<Node2D>("LedgeAnchor").GlobalPosition;
		_player.Sprite.Position = _spriteOriginalPosition;

		StateMachine.ChangeState(typeof(Idle));
	}
}

