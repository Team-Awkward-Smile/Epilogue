using Epilogue.Nodes;
using Godot;
using static Godot.GodotObject;

namespace Epilogue.Actors.TerraBischem.States;
/// <inheritdoc/>
public partial class Laugh : State
{
	/// <summary>
	///		State that allows the Terra Bischem to laught after hitting Hestmor
	/// </summary>
	/// <param name="stateMachine">State Machine that owns this State</param>
	public Laugh(StateMachine stateMachine) : base(stateMachine)
	{
	}

	internal override void OnEnter(params object[] args)
	{
		AnimPlayer.Play("laugh");
		AnimPlayer.Connect(AnimationMixer.SignalName.AnimationFinished, Callable.From((StringName animName) => StateMachine.ChangeState(typeof(Idle))), (uint)ConnectFlags.OneShot);
	}
}
