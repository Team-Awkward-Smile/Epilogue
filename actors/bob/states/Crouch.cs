using Epilogue.nodes;
using Godot;
using System.Threading.Tasks;

namespace Epilogue.actors.hestmor.states;
/// <summary>
///		State that allows Hestmor to crouch
/// </summary>
public partial class Crouch : PlayerState
{
	internal override void OnInput(InputEvent @event)
	{
		if(@event.IsActionReleased(CrouchInput))
		{
			StateMachine.ChangeState("Idle");
		}
	}

	internal override void OnEnter(params object[] args)
	{
		Player.CanChangeFacingDirection = false;

		AnimPlayer.Play("crouch");
	}

	internal override async Task OnLeaveAsync()
	{
		AnimPlayer.PlayBackwards("crouch");

		await ToSignal(AnimPlayer, "animation_finished");
	}
}
