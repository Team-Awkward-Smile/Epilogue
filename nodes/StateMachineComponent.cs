using Godot;
using System.Collections.Generic;
using System.Linq;

namespace Epilogue.nodes;
[GlobalClass]
public partial class StateMachineComponent : Node
{
	[Export] private StateComponent InitialState { get; set; }

	public bool CanInteract { get; set; } = true;

    private readonly HashSet<StateComponent> _states = new();

	private StateComponent _currentState;

	public override void _Ready()
	{
		_states.UnionWith(GetChildren().OfType<StateComponent>());

		if(InitialState is not null)
		{
			_currentState = InitialState;
		}
		else
		{
			GD.PushWarning($"Initial State of actor [{Owner.Name}] not set. Defaulting to [{_states.First().Name}]");
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

	public async void ChangeState(string stateName)
	{
		var oldState = _currentState;

		_currentState = null;

		var newState = _states.Where(s => s.Name == stateName).FirstOrDefault();

		if(newState is null)
		{
			GD.PushWarning($"State [{stateName}] not found");

			_currentState = oldState;

			return;
		}

		oldState.OnLeave();

		await oldState.OnLeaveAsync();

		_currentState = newState;
		_currentState.OnEnter();
	}
}
