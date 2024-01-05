using Epilogue.Nodes;

namespace Epilogue.Actors.Icarasia.States;
/// <inheritdoc/>
public partial class Stun : State
{
	private readonly float _baseStunDuration;

	private float _stunDuration;
	private float _timer;

	/// <summary>
	///		State used by the Icarasia when stunned
	/// </summary>
	/// <param name="stateMachine">State Machine who owns this State</param>
	/// <param name="stunDuration">Duration (in seconds) this State will remain active before changing itself</param>
	public Stun(StateMachine stateMachine, float stunDuration) : base(stateMachine)
	{
		_baseStunDuration = stunDuration;
	}

	// args[0] - float? - Stun Duration
	internal override void OnEnter(params object[] args)
	{
		AnimPlayer.Play("stun");

		_stunDuration = args.Length == 0 ? _baseStunDuration : (float)args[0];
	}

	internal override void PhysicsUpdate(double delta)
	{
		_timer += (float)delta;

		if (_timer >= _stunDuration)
		{
			StateMachine.ChangeState(typeof(Move));
		}
	}
}
