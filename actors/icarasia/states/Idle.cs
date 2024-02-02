using Epilogue.Nodes;
using Godot;

namespace Epilogue.Actors.Icarasia.States;
/// <inheritdoc/>
public partial class Idle : State
{
	private readonly Icarasia _icarasia;

	private float _timer;
	private float? _returnToWanderTimer;

	/// <summary>
	///		State used by the Icarasia while the player is not detected
	/// </summary>
	/// <param name="stateMachine">State Machine who owns this State</param>
	public Idle(StateMachine stateMachine) : base(stateMachine)
	{
		_icarasia = (Icarasia)stateMachine.Owner;
	}

	internal override void OnEnter(params object[] args)
	{
		_timer = 0f;

		if (args.Length > 0)
		{
			_returnToWanderTimer = (float)args[0];
		}
	}

	internal override void PhysicsUpdate(double delta)
	{
		// Returns to the "Wander" State and makes the Icarasia change it's direction
		if (_returnToWanderTimer is not null && (_returnToWanderTimer -= (float)delta) <= 0f)
		{
			StateMachine.ChangeState(typeof(Wander), true);

			return;
		}

		_timer += (float)delta * 10f;

		var waveX = Mathf.Sin(_timer) / 5f;
		var waveY = Mathf.Cos(_timer) / 5f;

		_icarasia.GlobalPosition += new Vector2(waveX, waveY);

		if (_icarasia.IsPlayerDetected)
		{
			StateMachine.ChangeState(typeof(Move));
		}
	}
}
