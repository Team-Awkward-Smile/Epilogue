using Epilogue.Nodes;

namespace Epilogue.Actors.MossPlant.States;
/// <inheritdoc/>
public partial class Idle : State
{
	private readonly MossPlant _mossPlant;

	/// <summary>
	///		State used by the Moss Plant of Guwama while the player is not within reach
	/// </summary>
	/// <param name="stateMachine">State Machine that owns this State</param>
	public Idle(StateMachine stateMachine) : base(stateMachine)
	{
		_mossPlant = (MossPlant)stateMachine.Owner;
	}

	internal override void Update(double delta)
	{
		if (_mossPlant.IsPlayerInRange)
		{
			StateMachine.ChangeState(typeof(Combat));
		}
	}
}
