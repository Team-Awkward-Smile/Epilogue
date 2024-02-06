using Epilogue.Extensions;
using Epilogue.Nodes;

namespace Epilogue.Actors.Icarasia.States;
///<inheritdoc/>
public partial class Vulnerable : State
{
	private readonly Icarasia _icarasia;

	private float _deathTimer;
	private float? _timer;

	/// <summary>
	///		State used by the Icarasia when becoming vulnerable
	/// </summary>
	/// <param name="stateMachine">State Machine who owns this State</param>
	public Vulnerable(StateMachine stateMachine) : base(stateMachine)
	{
		_icarasia = (Icarasia)stateMachine.Owner;
	}

	// args[0] - float? - Time to Die
	internal override void OnEnter(params object[] args)
	{
		_icarasia.Sprite.SetShaderMaterialParameter("vulnerabilityActive", true);

		if (args.Length > 0)
		{
			_timer = (float)args[0];
		}
	}

	internal override void PhysicsUpdate(double delta)
	{
		_deathTimer += (float)delta;

		if (_timer is not null && _deathTimer >= _timer)
		{
			StateMachine.ChangeState(typeof(Die));
		}
	}
}