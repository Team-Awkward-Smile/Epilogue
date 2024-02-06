using Epilogue.Nodes;

namespace Epilogue.Actors.rob.states;
/// <inheritdoc/>
public partial class Stun : State
{
	/// <summary>
	/// 	State to allow Rob to get stunned and stop acting
	/// </summary>
	/// <param name="stateMachine">The State Machine who owns this State</param>
	public Stun(StateMachine stateMachine) : base(stateMachine)
	{
	}

	internal override void OnEnter(params object[] args)
	{
		AnimPlayer.PlayBackwards("Combat/stun");
	}
}
