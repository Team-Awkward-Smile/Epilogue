using System.Threading.Tasks;
using Epilogue.Nodes;
using Godot;

namespace Epilogue.Actors.rob.states;
/// <inheritdoc/>
public partial class Stun : State
{
	private readonly Rob _rob;

	private float _timer = 0f;

	/// <summary>
	/// 	State to allow Rob to get stunned and stop acting
	/// </summary>
	/// <param name="stateMachine">The State Machine who owns this State</param>
	public Stun(StateMachine stateMachine) : base(stateMachine)
	{
		_rob = (Rob) stateMachine.Owner;
	}

	internal override void OnEnter(params object[] args)
	{
		AnimPlayer.PlayBackwards("Combat/stun");
	}
}
