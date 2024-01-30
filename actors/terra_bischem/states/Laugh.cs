using Epilogue.Nodes;
using Godot;
using static Godot.GodotObject;

namespace Epilogue.Actors.TerraBischem.States;
public partial class Laugh : State
{
	private readonly YoyoTerraBischem _terraBischem;

	public Laugh(StateMachine stateMachine) : base(stateMachine)
	{
		_terraBischem = (YoyoTerraBischem)stateMachine.Owner;
	}

	internal override void OnEnter(params object[] args)
	{
		AnimPlayer.Play("laugh");
		AnimPlayer.Connect(AnimationMixer.SignalName.AnimationFinished, Callable.From((StringName animName) => StateMachine.ChangeState(typeof(Idle))), (uint)ConnectFlags.OneShot);
	}
}
