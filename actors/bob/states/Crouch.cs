using Epilogue.nodes;
using Godot;

namespace Epilogue.actors.hestmor.states;
public partial class Crouch : StateComponent
{
	public override void OnEnter()
	{
		AnimPlayer.Play("Bob/Crouching");
	}

	public override void Update(double delta)
	{
		if(Input.IsActionJustReleased("crouch"))
		{
			StateMachine.ChangeState("Idle");
		}
	}

	public override void OnLeave()
	{
		AnimPlayer.PlayBackwards("Bob/Crouching");
		AnimPlayer.AnimationFinished += (StringName animName) => EmitSignal(SignalName.StateFinished);
	}
}
