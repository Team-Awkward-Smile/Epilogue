using Epilogue.extensions;
using Epilogue.nodes;
using Godot;

namespace Epilogue.actors.rob.states;
/// <inheritdoc/>
public partial class Wander : State
{
	private readonly Rob _rob;
	private readonly float _wanderSpeed;

	/// <summary>
	/// 	State that makes Rob move around the map randomly, waiting for a chance to approach the player again
	/// </summary>
	/// <param name="stateMachine">The State Machine who owns this State</param>
	/// <param name="wanderSpeed">The horizontal speed of Rob when wandering around the map</param>
	public Wander(StateMachine stateMachine, float wanderSpeed) : base(stateMachine)
	{
		_rob = (Rob) stateMachine.Owner;
		_wanderSpeed = wanderSpeed;
	}

	internal override async void OnEnter(params object[] args)
	{
		_rob.WaitingForNavigationQuery = true;

		var rng = new RandomNumberGenerator();

		await _rob.UpdatePathToWander(new Vector2(rng.RandfRange(-5000f, 5000f), rng.RandfRange(-5000f, 5000f)));

		_rob.WaitingForNavigationQuery = false;

		AnimPlayer.PlayBackwards("walk");
	}

	internal override void PhysicsUpdate(double delta)
	{
		if(_rob.WaitingForNavigationQuery)
		{
			return;
		}

		if(_rob.IsPlayerReachable)
		{
			StateMachine.ChangeState(typeof(Move));

			return;
		}

		_rob.Velocity = _rob.WanderNavigationAgent2D.GetNextVelocity(_rob.GlobalPosition, _wanderSpeed);
		_rob.MoveAndSlide();
	}
}
