using Epilogue.Global.Enums;
using Epilogue.Nodes;
using Godot;

namespace Epilogue.Actors.VafaKeleth.States;
public partial class TakeDamage : State
{
	private readonly VafaKeleth _vafaKeleth;

	private bool _knockBackActive = false;

	public TakeDamage(StateMachine stateMachine) : base(stateMachine)
	{
		_vafaKeleth = (VafaKeleth)stateMachine.Owner;
	}

	// args[0] - DamageType - Type of damage that caused this State to become active
	internal override void OnEnter(params object[] args)
	{
		var damageType = (DamageType)args[0];

		_knockBackActive = false;

		AnimPlayer.Play("take_damage");

		if (damageType != DamageType.Unarmed)
		{
			_vafaKeleth.RelativeVelocity = new Vector2(-30f, 0f);

			_knockBackActive = true;
		}

		AnimPlayer.AnimationFinished += (StringName anim) => StateMachine.ChangeState(typeof(Combat));
	}

	internal override void PhysicsUpdate(double delta)
	{
		if (_knockBackActive)
		{
			_vafaKeleth.MoveAndSlide();
		}
	}
}
