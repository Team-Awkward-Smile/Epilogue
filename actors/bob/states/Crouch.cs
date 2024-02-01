using Epilogue.Nodes;
using Godot;
using System.Threading.Tasks;

namespace Epilogue.actors.hestmor.states;
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
		_player = (Player) stateMachine.Owner;
	}

	internal override void OnInput(InputEvent @event)
	{
		if(@event.IsActionReleased("crouch"))
		{
			StateMachine.ChangeState(typeof(Idle));
		}
	}

	internal override void OnEnter(params object[] args)
	{
		_player.CanChangeFacingDirection = false;

		AnimPlayer.Play("crouch");
	}

	internal override async Task OnLeave()
	{
		AnimPlayer.PlayBackwards("crouch");

		await StateMachine.ToSignal(AnimPlayer, "animation_finished");
	}
}
