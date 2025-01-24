using Epilogue.Nodes;
using Godot;

namespace Epilogue.Actors.VafaKeleth.States;
/// <inheritdoc/>
public partial class Punch : State
{
	private readonly VafaKeleth _vafaKeleth;

	/// <summary>
	///		State that allows the Vafa'Keleth to perform a Punch attack
	/// </summary>
	/// <param name="stateMachine">State Machine that owns this State</param>
	public Punch(StateMachine stateMachine) : base(stateMachine)
	{
		_vafaKeleth = (VafaKeleth)stateMachine.Owner;
	}

	internal override void OnStateMachineActivation()
	{
		AnimPlayer.AnimationFinished += (StringName animationName) =>
		{
			if (!Active || animationName != "punch")
			{
				return;
			}

			StateMachine.ChangeState(typeof(Combat));
		};
	}

	internal override void OnEnter(params object[] args)
	{
		_vafaKeleth.CanAttemptSlide = false;

		AnimPlayer.Play("punch");
	}
}
