using Epilogue.Nodes;
using Godot;

namespace Epilogue.Actors.VafaKeleth.States;
public partial class Idle : State
{
	private readonly VafaKeleth _vafaKeleth;
	private readonly RandomNumberGenerator _rng;
	private readonly float _idleMinTime;
	private readonly float _idleMaxTime;
	private readonly float _detectionRange;

	private float _timer;
	private float _idleTime;
	private ShapeCast2D _attackShapeCast2D;

	public Idle(StateMachine stateMachine, float idleMinTime, float idleMaxTime, float detectionRange) : base(stateMachine)
	{
		_vafaKeleth = (VafaKeleth)stateMachine.Owner;
		_rng = new RandomNumberGenerator();
		_idleMinTime = idleMinTime;
		_idleMaxTime = idleMaxTime;
		_detectionRange = detectionRange;
	}

	internal override void OnEnter(params object[] args)
	{
		AnimPlayer.Play("idle");

		_timer = 0f;
		_idleTime = _rng.RandfRange(_idleMinTime, _idleMaxTime);
		_attackShapeCast2D = _vafaKeleth.ShapeCasts["Attack"];
	}

	internal override void PhysicsUpdate(double delta)
	{
		if ((_timer += (float)delta) >= _idleTime)
		{
			StateMachine.ChangeState(typeof(Wander));

			return;
		}

		if (_vafaKeleth.DistanceFromPlayer <= _detectionRange || _attackShapeCast2D.IsColliding())
		{
			StateMachine.ChangeState(typeof(Combat));

			return;
		}

		_vafaKeleth.Velocity = new Vector2(0f, _vafaKeleth.Velocity.Y + (StateMachine.Gravity * (float)delta));

		_vafaKeleth.MoveAndSlide();
	}
}
