using Epilogue.Nodes;
using Godot;

namespace Epilogue.Actors.VafaKeleth.States;
/// <inheritdoc/>
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

	/// <summary>
	///		State that allows the Vafa'Keleth to remain idle while not engaged in combat
	/// </summary>
	/// <param name="stateMachine">State Machine that owns this State</param>
	/// <param name="idleMinTime">Minimum time (in seconds) the Vafa'Keleth will remain still while walking back and forth</param>
	/// <param name="idleMaxTime">Maximum time (in seconds) the Vafa'Keleth will remain still while walking back and forth</param>
	/// <param name="detectionRange">Distance (in units) the player needs to be from the Vafa'Keleth to be detected</param>
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
		_vafaKeleth.CanAttemptSlide = false;

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
