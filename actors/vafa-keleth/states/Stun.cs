using Epilogue.Nodes;
using Godot;

namespace Epilogue.Actors.VafaKeleth.States;
/// <inheritdoc/>
public partial class Stun : State
{
	private readonly VafaKeleth _vafaKeleth;

	private float _exitTimer;

	/// <summary>
	///		State that allows the Vafa'Keleth to be stunned
	/// </summary>
	/// <param name="stateMachine">State Machine that owns this State</param>
	public Stun(StateMachine stateMachine) : base(stateMachine)
	{
		_vafaKeleth = (VafaKeleth)stateMachine.Owner;
	}

	// args[0] - float - Time (in seconds) before this State replaces itself with another one
	internal override void OnEnter(params object[] args)
	{
		_vafaKeleth.CanAttemptSlide = false;

		_exitTimer = (float)args[0];

		AnimPlayer.Play("idle");
	}

	internal override void Update(double delta)
	{
		if ((_exitTimer -= (float)delta) <= 0f)
		{
			StateMachine.ChangeState(typeof(Combat));
		}

		_vafaKeleth.Velocity = new Vector2(0f, _vafaKeleth.Velocity.Y + (StateMachine.Gravity * (float)delta));

		_vafaKeleth.MoveAndSlide();
	}
}
