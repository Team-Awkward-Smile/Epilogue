using Epilogue.nodes;
using Godot;
using System.Threading.Tasks;

namespace Epilogue.actors.hestmor.states;
public partial class Crouch : StateComponent
{
	public override void OnEnter()
	{
		AnimPlayer.Play("Bob/Crouching");
	}

	public override void Update(double delta)
	{
		if(Input.IsActionJustReleased(_crouchInput))
		{
			StateMachine.ChangeState("Idle");
		}
	}

	public override async Task OnLeaveAsync()
	{
		AnimPlayer.PlayBackwards("Bob/Crouching");

		await ToSignal(AnimPlayer, "animation_finished");

		EmitSignal(SignalName.StateFinished);
	}
}
