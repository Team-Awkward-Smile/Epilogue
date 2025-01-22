using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Epilogue.Nodes;
/// <summary>
///		Node that controls the State Machine of an Actor, defining it's current State and the code that will run
/// </summary>
public partial class StateMachine : Node
{
	/// <summary>
	///		Signal emitted when a new State is about to run it's <c>OnEnter()</c> logic
	/// </summary>
	/// <param name="newStateSpriteSheetId">The ID of the Sprite Sheet used by the State</param>
	[Signal] public delegate void StateEnteringEventHandler(int newStateSpriteSheetId);

	/// <summary>
	/// 	Signal emitted when a new State replaces another State (right after it runs its <c>OnEnter()</c> logic)
	/// </summary>
	[Signal] public delegate void StateEnteredEventHandler();

	/// <summary>
	/// 	Signal emitted when a State is replaced by another one (right after it runs its <c>OnLeave()</c>)
	/// </summary>
	[Signal] public delegate void StateExitedEventHandler();

	/// <summary>
	///		Controls whether this State Machine will become active as soon as it finishes loading
	/// </summary>
	[Export] public bool ActivateOnLoad { get; set; } = true;

	/// <summary>
	/// 	Value of the gravity affecting every State from this StateMachine
	/// </summary>
	public float Gravity { get; set; }

	private protected HashSet<State> _states = new();
	private protected State _currentState;

    private bool _canProcess = true;

    /// <summary>
    ///		Activates this State Machine and allow States to work
    /// </summary>
    public void Activate()
    {
        SetProcess(true);
        SetPhysicsProcess(true);

		foreach (State state in _states)
		{
			state.OnStateMachineActivation();
		}

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
        if (_canProcess)
        {
            _currentState?.Update(delta);
        }
    }

    /// <inheritdoc/>
    public override void _PhysicsProcess(double delta)
    {
        if (_canProcess)
        {
            _currentState?.PhysicsUpdate(delta);
        }
    }

    public State GetState()
    {
        return _currentState;
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
        _canProcess = false;

		var oldState = _currentState;
		var newState = _states.FirstOrDefault(s => s.GetType() == newStateType);

		if (newState is null)
		{
			GD.PushWarning($"State [{newStateType}] not found");

            _currentState = oldState;

            return;
        }

		oldState.Deactivating = true;

		await oldState.OnLeave();

		oldState.Deactivating = false;
		oldState.Active = false;

		EmitSignal(SignalName.StateExited);

		newState.Active = true;

		_currentState = newState;

		EmitSignal(SignalName.StateEntering, _currentState.SpriteSheetId);

		_currentState.OnEnter(args);

		EmitSignal(SignalName.StateEntered);

        _canProcess = true;
	}
}
