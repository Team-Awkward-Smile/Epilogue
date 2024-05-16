using Epilogue.Global.Enums;
using Epilogue.Nodes;
using Godot;

namespace Epilogue.Actors.VafaKeleth.States;
/// <inheritdoc/>
public partial class TakeDamage : State
{
	private readonly VafaKeleth _vafaKeleth;

	private double? _stunTimer;
	private bool _exitOnAnimationFinish = false;

	/// <summary>
	///		State that allows the Vafa'Keleth to react to damage taken
	/// </summary>
	/// <param name="stateMachine">State Machine that owns this State</param>
	public TakeDamage(StateMachine stateMachine) : base(stateMachine)
	{
		_vafaKeleth = (VafaKeleth)stateMachine.Owner;
	}

	internal override void OnStateMachineActivation()
	{
		AnimPlayer.AnimationFinished += (StringName animationName) =>
		{
			if (!Active || !_exitOnAnimationFinish || animationName != "take_damage")
			{
				return;
			}

			StateMachine.ChangeState(typeof(Combat));
		};
	}

	// args[0] - DamageType - Type of damage that caused this State to become active
	internal override void OnEnter(params object[] args)
	{
		_vafaKeleth.CanAttemptSlide = false;

		_exitOnAnimationFinish = false;

		var damageType = (DamageType)args[0];

		AnimPlayer.Play("take_damage");

		if (damageType != DamageType.Unarmed)
		{
			_vafaKeleth.RelativeVelocity = new Vector2(-30f, 0f);

			_stunTimer = null;
			_exitOnAnimationFinish = true;
		}
		else
		{
			_stunTimer = 0.2;
		}
	}

	internal override void PhysicsUpdate(double delta)
	{
		if (_stunTimer is not null && (_stunTimer -= delta) <= 0)
		{
			StateMachine.ChangeState(typeof(Combat));
		}

		_vafaKeleth.Velocity = new Vector2(_vafaKeleth.Velocity.X, _vafaKeleth.Velocity.Y + (StateMachine.Gravity * (float)delta));

		_vafaKeleth.MoveAndSlide();
	}
}
