using Epilogue.nodes;

namespace Epilogue.actors.rob.states;
/// <summary>
///		State to allow Rob to get stunned and stop acting
/// </summary>
public partial class Stun : NpcState
{
	internal override void OnEnter(params object[] args)
	{
		AnimPlayer.PlayBackwards("Combat/stun");

		GetTree().CreateTimer(Npc.CustomVariables["StunTimer"].AsSingle()).Timeout += () => StateMachine.ChangeState("Move");
	}

	internal override void OnLeave()
	{
		Npc.IsStunned = false;
	}
}
