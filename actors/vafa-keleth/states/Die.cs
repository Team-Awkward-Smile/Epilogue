using Epilogue.Nodes;
using Godot;

namespace Epilogue.Actors.VafaKeleth.States;
/// <inheritdoc/>
public partial class Die : State
{
	private readonly VafaKeleth _vafaKeleth;

	/// <summary>
	///		State that allows the Vafa'Keleth to die
	/// </summary>
	/// <param name="stateMachine">State Machine that owns this State</param>
	public Die(StateMachine stateMachine) : base(stateMachine)
	{
		_vafaKeleth = (VafaKeleth)stateMachine.Owner;
	}

	internal override void OnStateMachineActivation()
	{
		AnimPlayer.AnimationFinished += (StringName animationName) =>
		{
			if (!Active || animationName != "die")
			{
				return;
			}

			_vafaKeleth.QueueFree();
		};
	}

	internal override void OnEnter(params object[] args)
	{
		_vafaKeleth.CanAttemptSlide = false;

		AnimPlayer.Play("die");
	}
}
