using Epilogue.Nodes;
using Godot;

namespace Epilogue.Actors.VafaKeleth.States;
public partial class Die : State
{
	private readonly VafaKeleth _vafaKeleth;

	public Die(StateMachine stateMachine) : base(stateMachine)
	{
		_vafaKeleth = (VafaKeleth)stateMachine.Owner;
	}

	internal override void OnEnter(params object[] args)
	{
		AnimPlayer.Play("die");

		AnimPlayer.AnimationFinished += (StringName anim) => _vafaKeleth.QueueFree();
	}
}
