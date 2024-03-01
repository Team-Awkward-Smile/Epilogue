using Epilogue.Nodes;
using Godot;

namespace Epilogue.Actors.VafaKeleth.States;
public partial class SpitFire : State
{
	private readonly VafaKeleth _vafaKeleth;

	public SpitFire(StateMachine stateMachine) : base(stateMachine)
	{
		_vafaKeleth = (VafaKeleth)stateMachine.Owner;
	}

	// args[0] - int - Angle to fire the stream
	internal override void OnEnter(params object[] args)
	{
		var fireAngle = (int)args[0];

		AnimPlayer.Play("spit_fire_" + (fireAngle == 0 ? "front" : "down"));

		AnimPlayer.AnimationFinished += (StringName anim) => StateMachine.ChangeState(typeof(Combat));
	}
}
