using Epilogue.Nodes;
using Godot;

namespace Epilogue.Actors.VafaKeleth.States;
public partial class Punch : State
{
	public Punch(StateMachine stateMachine) : base(stateMachine)
	{
	}

	internal override void OnEnter(params object[] args)
	{
		AnimPlayer.Play("punch");

		AnimPlayer.AnimationFinished += (StringName anim) => StateMachine.ChangeState(typeof(Combat));
	}
}
