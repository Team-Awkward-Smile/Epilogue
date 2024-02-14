using Epilogue.Nodes;
using Godot;

namespace Epilogue.Actors.MossPlant.States;
/// <inheritdoc/>
public partial class Shoot : State
{
	private readonly MossPlant _mossPlant;
	private readonly PackedScene _projectileScene;
	private readonly Node2D _projectileSpawnPoint;

	/// <summary>
	///		State used by the Moss Plant of Guwama to shoot a projectile towards the player
	/// </summary>
	/// <param name="stateMachine">State Machine that owns this State</param>
	public Shoot(StateMachine stateMachine) : base(stateMachine)
	{
		_mossPlant = (MossPlant)stateMachine.Owner;
		_projectileScene = GD.Load<PackedScene>("res://actors/moss_plant/projectile/moss_plant_projectile.tscn");
		_projectileSpawnPoint = _mossPlant.GetNode<Node2D>("ProjectileSpawnPoint");
	}

	internal override void OnEnter(params object[] args)
	{
		var projectile = _projectileScene.Instantiate() as MossPlantProjectile;

		_mossPlant.AttackTimer = 0f;
		_mossPlant.IsProjectileActive = true;

		_mossPlant.AddChild(projectile);

		projectile.GlobalTransform = _projectileSpawnPoint.GlobalTransform;

		StateMachine.ChangeState(typeof(Combat));
	}
}
