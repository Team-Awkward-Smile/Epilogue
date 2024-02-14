using Epilogue.Nodes;

namespace Epilogue.Actors.MossPlant.States;
/// <inheritdoc/>
public partial class Combat : State
{
	private readonly MossPlant _mossPlant;
	private readonly float _projectileCooldown;

	/// <summary>
	///		State used by the Moss Plant of Guwama to fight the player
	/// </summary>
	/// <param name="stateMachine">State Machine that owns this State</param>
	/// <param name="projectileCooldown">Time (in seconds) before another projectile can be shot</param>
	public Combat(StateMachine stateMachine, float projectileCooldown) : base(stateMachine)
	{
		_mossPlant = (MossPlant)stateMachine.Owner;
		_projectileCooldown = projectileCooldown;
	}

	internal override void Update(double delta)
	{
		if (!_mossPlant.IsPlayerInRange)
		{
			StateMachine.ChangeState(typeof(Idle));
		}
		else if (_mossPlant.AttackTimer >= _projectileCooldown && !_mossPlant.IsProjectileActive)
		{
			StateMachine.ChangeState(typeof(Shoot));
		}
	}
}
