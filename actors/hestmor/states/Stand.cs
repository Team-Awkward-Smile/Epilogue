using Epilogue.Nodes;
using Godot;
using System;
using System.Threading.Tasks;

namespace Epilogue.Actors.Hestmor.States;
/// <inheritdoc/>
public partial class Stand : State
{
	/// <summary>
	///		State that allows Hestmor to leave a Crouch or Crawl State
	/// </summary>
	/// <param name="stateMachine"></param>
	public Stand(StateMachine stateMachine) : base(stateMachine)
	{
	}

	internal override void OnEnter(params object[] args)
	{
		StateMachine.ChangeState(typeof(Idle));
	}

	internal override async Task OnLeave()
	{
		AnimPlayer.Play("Crouch/crouch_end", customSpeed: 2f);

		await StateMachine.ToSignal(AnimPlayer, "animation_finished");
	}
}
