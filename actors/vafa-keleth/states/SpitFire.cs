using Epilogue.Nodes;
using Godot;

namespace Epilogue.Actors.VafaKeleth.States;
/// <inheritdoc/>
public partial class SpitFire : State
{
	private readonly VafaKeleth _vafaKeleth;

	/// <summary>
	///		State that allows the Vafa'Keleth to perform a Spit Fire attack
	/// </summary>
	/// <param name="stateMachine">State Machine that owns this State</param>
	public SpitFire(StateMachine stateMachine) : base(stateMachine)
	{
		_vafaKeleth = (VafaKeleth)stateMachine.Owner;
	}

	internal override void OnStateMachineActivation()
	{
		AnimPlayer.AnimationFinished += (StringName animationName) =>
		{
			if (!Active || !animationName.ToString().StartsWith("spit_fire"))
			{
				return;
			}

			StateMachine.ChangeState(typeof(Combat));
		};
	}

	// args[0] - int - Angle to fire the stream
	internal override void OnEnter(params object[] args)
	{
		_vafaKeleth.CanAttemptSlide = false;

		var fireAngle = (int)args[0];

		AnimPlayer.Play("spit_fire_" + (fireAngle == 0 ? "front" : "down"));
	}
}
