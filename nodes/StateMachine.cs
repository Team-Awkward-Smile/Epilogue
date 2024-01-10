using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Epilogue.nodes;
/// <summary>
///		Node that controls the State Machine of an Actor, defining it's current State and the code that will run
/// </summary>
public partial class StateMachine : Node
{
	/// <summary>
	///		Defines if the current State allows the Actor to interact with the world
	/// </summary>
	public bool CanInteract { get; set; } = true;

	/// <summary>
	/// 	Value of the gravity affecting every State from this StateMachine
	/// </summary>
	public float Gravity { get; set; }

    private protected HashSet<State> _states = new();
	private protected State _currentState;

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
		SetProcess(false);
		SetPhysicsProcess(false);

		Gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();
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
    /// <param name="newStateType">Type of the new State that will replace the current one</param>
    /// <param name="args">Optional list of arguments that may be used by specific States</param>
	public async void ChangeState(Type newStateType, params object[] args)
	{
		var oldState = _currentState;

		_currentState = null;

		var newState = _states.Where(s => s.GetType() == newStateType).FirstOrDefault();

		if(newState is null)
		{
			GD.PushWarning($"State [{newStateType}] not found");

			_currentState = oldState;

			return;
		}

		await oldState.OnLeave();

		_currentState = newState;
		_currentState.OnEnter(args);
	}
}
