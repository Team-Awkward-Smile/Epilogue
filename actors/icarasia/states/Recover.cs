using Epilogue.Nodes;
using Godot;

namespace Epilogue.Actors.Icarasia.States;
/// <inheritdoc/>
public partial class Recover : State
{
	private readonly Icarasia _icarasia;
	private readonly float _recoveryTime;

	private float _timer;

	/// <summary>
	/// 	State used to stop moving and recover after missing a Charge Attack
	/// </summary>
	/// <param name="stateMachine">The State Machine who owns this State</param>
	/// <param name="recoveryTime">The time (in seconds) the Icarasia will wait before Charging again</param>
	public Recover(StateMachine stateMachine, float recoveryTime) : base(stateMachine)
	{
		_icarasia = (Icarasia)stateMachine.Owner;
		_recoveryTime = recoveryTime;
	}

	internal override void OnEnter(params object[] args)
	{
		_icarasia.Velocity = Vector2.Zero;
		_icarasia.MoveAndSlide();
		_timer = 0f;
	}

	internal override void PhysicsUpdate(double delta)
	{
		_timer += (float)delta;

		if (_timer >= _recoveryTime)
		{
			StateMachine.ChangeState(typeof(Charge));
		}
	}
}
