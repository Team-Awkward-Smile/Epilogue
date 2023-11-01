using Godot;
using System.Collections.Generic;
using System.Linq;

namespace Epilogue.nodes;
/// <summary>
///		Node that controls the State Machine of an Actor, defining it's current State and the code that will run
/// </summary>
[GlobalClass, Tool, Icon("res://nodes/icons/state_machine.png")]
public partial class StateMachine : Node
{
	/// <summary>
	///		Defines if the current State allows the Actor to interact with the world
	/// </summary>
	public bool CanInteract { get; set; } = true;

    private readonly HashSet<State> _states = new();

	private State _currentState;

	/// <summary>
	///		Activates this State Machine and allow States to work
	/// </summary>
	public void Activate()
	{
		SetProcess(true);
		SetPhysicsProcess(true);

		_currentState.OnEnter();
	}

	/// <inheritdoc/>
	public override void _Ready()
	{
		if(Engine.IsEditorHint())
		{
			return;
		}

		SetProcess(false);
		SetPhysicsProcess(false);

		_states.UnionWith(GetChildren().OfType<State>());
		_currentState = _states.First();
	}

	/// <summary>
	///		Sends the input event to the currently active State
	/// </summary>
	/// <param name="event">The input event to send to the active State</param>
	public void PropagateInputToState(InputEvent @event)
	{
		_currentState?.OnInput(@event);
	}

	/// <inheritdoc/>
	public override void _Process(double delta)
	{
		_currentState?.Update(delta);
	}

	/// <inheritdoc/>
	public override void _PhysicsProcess(double delta)
	{
		_currentState?.PhysicsUpdate(delta);
	}

	/// <summary>
	///		Changes the current State of the Actor. 
	///		If the informed State is valid, the methods <c>OnLeave</c> and <c>OnLeaveAsync</c> of the current State will be called.
	///		Then the State will be replaced by the new one, and the method <c>OnEnter</c> of the new State will be called
	/// </summary>
	/// <param name="stateName">Name of the new PlayerState</param>
	public async void ChangeState(string stateName, params object[] args)
	{
		var oldState = _currentState;

		_currentState = null;

		var newState = _states.Where(s => s.Name == stateName).FirstOrDefault();

		if(newState is null)
		{
			GD.PushWarning($"PlayerState [{stateName}] not found");

			_currentState = oldState;

			return;
		}

		oldState.OnLeave();

		await oldState.OnLeaveAsync();

		_currentState = newState;
		_currentState.OnEnter(args);
	}
}
