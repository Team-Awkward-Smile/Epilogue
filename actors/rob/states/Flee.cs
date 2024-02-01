using Epilogue.extensions;
using Epilogue.Nodes;
using Godot;

namespace Epilogue.actors.rob.states;
/// <inheritdoc/>
public partial class Flee : State
{
	private readonly Rob _rob;
	private readonly float _fleeSpeed;

	/// <summary>
	/// 	State that makes Rob move away from the player
	/// </summary>
	/// <param name="stateMachine">The State Machine who owns this State</param>
	/// <param name="fleeSpeed">The horizontal speed of Rob when fleeing from Hestmor</param>
	public Flee(StateMachine stateMachine, float fleeSpeed) : base(stateMachine)
	{
		_rob = (Rob) stateMachine.Owner;
		_fleeSpeed = fleeSpeed;
	}

	internal override void OnEnter(params object[] args)
	{
		StateMachine.GetTree().CreateTimer(_rob.FleeDuration).Timeout += () => StateMachine.ChangeState(typeof(Move));
		AnimPlayer.Play("walk", -1, 2, true);
	}

	internal override void PhysicsUpdate(double delta)
	{
		var velocity = _rob.PlayerNavigationAgent2D.GetNextVelocity(_rob.GlobalPosition, _fleeSpeed) * -1;

		_rob.Velocity = new Vector2(velocity.X, velocity.Y + StateMachine.Gravity * (float) delta);
		_rob.MoveAndSlide();
	}
}
