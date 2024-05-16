using Epilogue.Nodes;
using Godot;

namespace Epilogue.Actors.VafaKeleth.States;
/// <inheritdoc/>
public partial class Slide : State
{
	private readonly VafaKeleth _vafaKeleth;
	private readonly float _slideSpeed;

	/// <summary>
	///		State that allows the Vafa'Keleth to perform a slide
	/// </summary>
	/// <param name="stateMachine">State Machine that owns this State</param>
	/// <param name="slideSpeed">Speed (in units) of the slide</param>
	public Slide(StateMachine stateMachine, float slideSpeed) : base(stateMachine)
	{
		_vafaKeleth = (VafaKeleth)stateMachine.Owner;
		_slideSpeed = slideSpeed;
	}

	internal override void OnStateMachineActivation()
	{
		AnimPlayer.AnimationFinished += (StringName animationName) =>
		{
			if (!Active || animationName != "slide")
			{
				return;
			}

			StateMachine.ChangeState(typeof(Combat));
		};
	}

	internal override void OnEnter(params object[] args)
	{
		_vafaKeleth.CanAttemptSlide = false;
		_vafaKeleth.RelativeVelocity = new Vector2(_slideSpeed, _vafaKeleth.Velocity.Y);

		AnimPlayer.Play("slide");
	}

	internal override void PhysicsUpdate(double delta)
	{
		_vafaKeleth.Velocity = new Vector2(_vafaKeleth.Velocity.X, _vafaKeleth.Velocity.Y + (StateMachine.Gravity * (float)delta));

		_vafaKeleth.MoveAndSlide();
	}
}
