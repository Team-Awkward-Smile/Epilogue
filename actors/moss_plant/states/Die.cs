using Epilogue.Nodes;

namespace Epilogue.Actors.MossPlant.States;
/// <inheritdoc/>
public partial class Die : State
{
	private readonly MossPlant _mossPlant;

	/// <summary>
	///		State that allows the Moss Plant of Guwama to die
	/// </summary>
	/// <param name="stateMachine">State Machine that owns this State</param>
	public Die(StateMachine stateMachine) : base(stateMachine)
	{
		_mossPlant = (MossPlant)stateMachine.Owner;
	}

	internal override void OnEnter(params object[] args)
	{
		_mossPlant.QueueFree();
	}
}
