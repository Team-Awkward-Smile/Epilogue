using Epilogue.Nodes;
using Godot;
using System.Threading.Tasks;

namespace Epilogue.Actors.Hestmor.States;
/// <inheritdoc/>
public partial class Crouch : State
{
	private readonly Player _player;

	/// <summary>
	/// 	State that allows Hestmor to crouch
	/// </summary>
	/// <param name="stateMachine">The State Machine who owns this State</param>
	public Crouch(StateMachine stateMachine) : base(stateMachine)
	{
		_player = (Player)stateMachine.Owner;

		SpriteSheetId = (int)Enums.SpriteSheetId.Bob;
	}

	internal override void OnInput(InputEvent @event)
	{
		if (@event.IsActionReleased("crouch", true))
		{
			StateMachine.ChangeState(typeof(Idle));
		}
	}

	internal override void OnEnter(params object[] args)
	{
		_player.CanChangeFacingDirection = false;

		AnimPlayer.Play("crouch");
		AudioPlayer.PlayGenericSfx("Crouch2");
	}

	internal override async Task OnLeave()
	{
		AnimPlayer.PlayBackwards("crouch");

		await StateMachine.ToSignal(AnimPlayer, "animation_finished");
	}
}
