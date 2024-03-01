using Epilogue.Extensions;
using Epilogue.Nodes;
using Godot;
using static Godot.GodotObject;

namespace Epilogue.Actors.VafaKeleth.States;
public partial class Wander : State
{
	private readonly VafaKeleth _vafaKeleth;
	private readonly RandomNumberGenerator _rng;
	private readonly float _idleSpeed;
	private readonly float _wanderSpeed;

	public Wander(StateMachine stateMachine, float idleSpeed, float wanderSpeed) : base(stateMachine)
	{
		_vafaKeleth = (VafaKeleth)stateMachine.Owner;
		_rng = new RandomNumberGenerator();
		_idleSpeed = idleSpeed;
		_wanderSpeed = wanderSpeed;
	}

	internal override async void OnEnter(params object[] args)
	{
		var targetPosition = _vafaKeleth.GlobalPosition + new Vector2(_rng.RandfRange(50f, 120f) * (_rng.RandiRange(0, 1) == 1 ? 1 : -1), 0f);

		await _vafaKeleth.UpdatePathToWander(targetPosition);

		_vafaKeleth.WanderNavigationAgent2D.Connect(NavigationAgent2D.SignalName.NavigationFinished, Callable.From(() => StateMachine.ChangeState(typeof(Idle))), (uint)ConnectFlags.OneShot);
		_vafaKeleth.TurnTowards(targetPosition);

		AnimPlayer.Play("walk");
	}

	internal override void PhysicsUpdate(double delta)
	{
		if (_vafaKeleth.WaitingForNavigationQuery)
		{
			return;
		}

		_vafaKeleth.Velocity = _vafaKeleth.WanderNavigationAgent2D.GetNextVelocity(_vafaKeleth.GlobalPosition, 50f) + new Vector2(0f, _vafaKeleth.Velocity.Y + StateMachine.Gravity * (float)delta);

		_vafaKeleth.MoveAndSlide();
	}
}
