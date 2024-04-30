using Epilogue.Nodes;

namespace Epilogue.Actors.VafaKeleth.States;
/// <inheritdoc/>
public partial class Vulnerable : State
{
	private readonly VafaKeleth _vafaKeleth;

	private double _exitTime;

	/// <summary>
	///		State that allows the Vafa'Keleth to become Vulnerable
	/// </summary>
	/// <param name="stateMachine">State Machine that owns this State</param>
	public Vulnerable(StateMachine stateMachine) : base(stateMachine)
	{
		_vafaKeleth = (VafaKeleth)stateMachine.Owner;
	}

	// args[0] - double - Time (in seconds) the Vafa'Keleth will remain vulnerable before triggering it's desperation
	internal override void OnEnter(params object[] args)
	{
		_exitTime = (double)args[0];

		AnimPlayer.Play("idle");
	}

	internal override void Update(double delta)
	{
		if ((_exitTime -= delta) <= 0)
		{
			_vafaKeleth.TriggerDesperation();
		}
	}
}
