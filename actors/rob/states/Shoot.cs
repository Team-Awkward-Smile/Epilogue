using Epilogue.Extensions;
using Epilogue.Nodes;
using Godot;

namespace Epilogue.Actors.rob.states;
/// <inheritdoc/>
public partial class Shoot : State
{
	private readonly Rob _rob;
	private readonly Player _player;

	/// <summary>
	/// 	State that allows Rob to shoot the player
	/// </summary>
	/// <param name="stateMachine">The State Machine who owns this State</param>
	/// <param name="player">A reference to the player character</param>
	public Shoot(StateMachine stateMachine, Player player) : base(stateMachine)
	{
		_rob = (Rob) stateMachine.Owner;
		_player = player;
	}

	internal override void OnEnter(params object[] args)
	{
		_rob.TurnTowards(_player);

		AnimPlayer.PlayBackwards("Combat/shoot");
		AnimPlayer.AnimationFinished += OnAnimationFinish;
	}

	private void OnAnimationFinish(StringName animName)
	{
		AnimPlayer.AnimationFinished -= OnAnimationFinish;
		StateMachine.ChangeState(typeof(Move));
	}

	/// <summary>
	/// 	Spawns Rob's projectile at the position of the ProjectileSpawn Node
	/// </summary>
	public void SpawnProjectile()
	{
		var _projectile = GD.Load<PackedScene>("res://actors/rob/projectiles/projectile.tscn").Instantiate() as Projectile;
		
		StateMachine.GetTree().Root.AddChild(_projectile);

		_projectile.GlobalTransform = StateMachine.GetNode<Node2D>("../../FlipRoot/ProjectileSpawn").GlobalTransform;
	}
}
