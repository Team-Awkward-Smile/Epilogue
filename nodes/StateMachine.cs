using Godot;
using System.Collections.Generic;
using System.Linq;

namespace Epilogue.nodes;
/// <summary>
///		Node that controls the PlayerState Machine of an Actor, defining it's current PlayerState and the code that will run
/// </summary>
[GlobalClass]
public partial class StateMachine : Node
{
	/// <summary>
	///		The PlayerState this Actor will use when first spawned. If null, the initial PlayerState will be set to the first PlayerState belonging to the Actor
	/// </summary>
	[Export] private PlayerState InitialState { get; set; }


	private readonly HashSet<State> _states = new();

	private State _currentState;

	public override void _Ready()
	{
		_states.UnionWith(GetChildren().OfType<State>());

		if(InitialState is not null)
		{
			_currentState = InitialState;
		}
		else
		{
			GD.PushWarning($"Initial PlayerState of actor [{Owner.Name}] not set. Defaulting to [{_states.First().Name}]");
			_currentState = _states.First();
		}

		_currentState.OnEnter();
	}

	public override void _UnhandledInput(InputEvent @event)
	{
		_currentState?.OnInput(@event);
	}

	public override void _Process(double delta)
	{
		_currentState?.Update(delta);
	}

	public override void _PhysicsProcess(double delta)
	{
		_currentState?.PhysicsUpdate(delta);
	}

	/// <summary>
	///		Changes the current PlayerState of the Actor. 
	///		If the informed PlayerState is valid, the methods <c>OnLeave</c> and <c>OnLeaveAsync</c> of the current PlayerState will be called.
	///		Then the PlayerState will be replaced by the new one, and the method <c>OnEnter</c> of the new PlayerState will be called
	/// </summary>
	/// <param name="stateName">Name of the new PlayerState</param>
	public async void ChangeState(string stateName)
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
		_currentState.OnEnter();
	}
}
