using Godot;
using System.Linq;

namespace Epilogue.nodes;
public partial class StateComponent : Node
{
	[Signal] public delegate void StateStartedEventHandler();
	[Signal] public delegate void StateFinishedEventHandler();

	protected StateMachineComponent StateMachine { get; private set; }
	protected CharacterBody2D Character { get; private set; }
	protected AnimationPlayer AnimPlayer { get; private set; }
	protected float Gravity { get; private set; }

	public override void _Ready()
	{
		StateMachine = (StateMachineComponent) GetParent();
		Character = (CharacterBody2D) StateMachine.GetParent();
		AnimPlayer = Character.GetChildren().OfType<AnimationPlayer>().FirstOrDefault();
		Gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();

		if(StateMachine is null)
		{
			GD.PrintErr($"State Machine not found for State [{Name}]");
		}

		if(AnimPlayer is null)
		{
			GD.PrintErr($"Animation Player not found for State [{Name}]");
		}
	}

	public virtual void Update(double delta) { }

	public virtual void PhysicsUpdate(double delta) { }

	public virtual void OnEnter() => EmitSignal(SignalName.StateStarted);

	public virtual void OnLeave() => EmitSignal(SignalName.StateFinished);

	public virtual void OnInput(InputEvent @event) { }
}
