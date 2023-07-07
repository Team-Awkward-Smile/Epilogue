// meta-description: Base template for Node representing a State for an actor

using _BINDINGS_NAMESPACE_;
using Epilogue.nodes;

namespace Epilogue.actors.XXXXXX.states;
public partial class _CLASS_ : StateComponent
{
	// Called whenever an unhandled input is detected
	public override void OnInput(InputEvent @event)
	{
		base.OnInput(@event);
	}

	// Called when this state becomes active
	public override void OnEnter()
	{
		EmitSignal(SignalName.StateStarted);
		base.OnEnter();
	}

	// Called once per frame
	public override void Update(double delta)
	{
		base.Update(delta);
	}

	// Called once per logic frame (60 times per second, by default)
	public override void PhysicsUpdate(double delta)
	{
		base.PhysicsUpdate(delta);
	}

	// Called when this state is replaced by another one
	public override void OnLeave()
	{
		base.OnLeave();
		EmitSignal(SignalName.StateFinished);
	}
}
