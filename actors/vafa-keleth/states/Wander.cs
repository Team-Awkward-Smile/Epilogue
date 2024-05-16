using Epilogue.Extensions;
using Epilogue.Nodes;
using Godot;

namespace Epilogue.Actors.VafaKeleth.States;
/// <inheritdoc/>
public partial class Wander : State
{
	private readonly VafaKeleth _vafaKeleth;
	private readonly RandomNumberGenerator _rng;

	/// <summary>
	///		State that allows the Vafa'Keleth to walk to a random position if the player is unreachable
	/// </summary>
	/// <param name="stateMachine"></param>
	public Wander(StateMachine stateMachine) : base(stateMachine)
	{
		_vafaKeleth = (VafaKeleth)stateMachine.Owner;
		_rng = new RandomNumberGenerator();
	}

	internal override void OnStateMachineActivation()
	{
		_vafaKeleth.WanderNavigationAgent2D.NavigationFinished += () =>
		{
			if (!Active)
			{
				return;
			}

			StateMachine.ChangeState(typeof(Idle));
		};
	}


	internal override async void OnEnter(params object[] args)
	{
		_vafaKeleth.CanAttemptSlide = true;

		var targetPosition = _vafaKeleth.GlobalPosition + new Vector2(_rng.RandfRange(50f, 120f) * (_rng.RandiRange(0, 1) == 1 ? 1 : -1), 0f);

		await _vafaKeleth.UpdatePathToWander(targetPosition);

		_vafaKeleth.TurnTowards(targetPosition);

		AnimPlayer.Play("walk");
	}

	internal override void PhysicsUpdate(double delta)
	{
		if (_vafaKeleth.WaitingForNavigationQuery)
		{
			return;
		}

		_vafaKeleth.Velocity = _vafaKeleth.WanderNavigationAgent2D.GetNextVelocity(_vafaKeleth.GlobalPosition, 50f) + new Vector2(0f, _vafaKeleth.Velocity.Y + (StateMachine.Gravity * (float)delta));

		_vafaKeleth.MoveAndSlide();
	}
}
