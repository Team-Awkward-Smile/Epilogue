using Epilogue.Extensions;
using Epilogue.Nodes;

namespace Epilogue.Actors.Icarasia.States;
///<inheritdoc/>
public partial class Vulnerable : State
{
	private readonly Icarasia _icarasia;

	private double _vulnerableTimer;
	private double? _deathTimer;

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
			_deathTimer = (double)args[0];
		}
	}

	internal override void PhysicsUpdate(double delta)
	{
		_vulnerableTimer += delta;

		if (_deathTimer is not null && _vulnerableTimer >= _deathTimer)
		{
			StateMachine.ChangeState(typeof(Die));

			return;
		}

		if (_vulnerableTimer > 3f)
		{
			StateMachine.ChangeState(typeof(Charge));
		}
	}
}