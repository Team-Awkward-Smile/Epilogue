using Epilogue.Extensions;
using Epilogue.Nodes;
using Godot;

namespace Epilogue.Actors.Icarasia.States;
/// <inheritdoc/>
public partial class Shoot : State
{
	private readonly Icarasia _icarasia;
	private readonly Node2D _projectilePivot;
	private readonly PackedScene _projectileScene;

	private int _angle;

	/// <summary>
	///		State used by the Icarasia to shoot at the player
	/// </summary>
	/// <param name="stateMachine">State Machine who owns this State</param>
	public Shoot(StateMachine stateMachine) : base(stateMachine)
	{
		_icarasia = (Icarasia)stateMachine.Owner;
		_projectilePivot = _icarasia.GetNode<Node2D>("FlipRoot/ProjectilePivot");
		_projectileScene = GD.Load<PackedScene>("res://actors/icarasia/projectiles/projectile.tscn");
	}

	// args[0] - int - Shot Angle (in degrees)
	internal override void OnEnter(params object[] args)
	{
		_angle = (int)args[0];

		_projectilePivot.RotationDegrees = _angle;

		var projectile = _projectileScene.Instantiate() as Projectile;

		StateMachine.GetTree().GetLevel().AddChild(projectile);

		projectile.GlobalTransform = _projectilePivot.GetNode<Node2D>("ProjectileSpawn").GlobalTransform;

		_icarasia.AttackTimer = 0f;
		_icarasia.ShotAngle = null;

		StateMachine.ChangeState(typeof(Move));
	}
}
