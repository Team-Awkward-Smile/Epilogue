using Epilogue.Actors.VafaKeleth.States;
using Epilogue.Global.Enums;
using Epilogue.Nodes;
using Godot;
using Godot.Collections;
using System;

namespace Epilogue.Actors.VafaKeleth;
public partial class VafaKeleth : Npc
{
	/// <inheritdoc/>
	public override Dictionary<DamageType, float> DamageModifiers { get; set; } = new()
	{
		{ DamageType.Water, 2f },
		{ DamageType.Spread, 0.5f }
	};

	private protected override bool UseDefaultPathfinding => true;

	public double TimeSinceLastAttack { get; set; }
	public int[] AttackAngles { get; private set; } = new int[] { 0, 90 };

	private AnimationPlayer _fireStreamAnimationPlayer;
	private Node2D _fireStreamRoot;

	public override void _Ready()
	{
		base._Ready();

		_fireStreamAnimationPlayer = GetNode<AnimationPlayer>("%FireStreamAnimationPlayer");
		_fireStreamRoot = GetNode<Node2D>("%FireStreamRoot");
	}

	private protected override void OnDamageTaken(float damage, float currentHp, DamageType damageType)
	{
		AttackAngles = new int[] { 0 };
		PlayerNavigationAgent2D.TargetDesiredDistance = 25f;
		PlayerNavigationAgent2D.TargetPosition = Player.GlobalPosition;
		ShapeCasts["Attack"].TargetPosition = new Vector2(20f, 0f);

		_npcStateMachine.ChangeState(typeof(TakeDamage), damageType);
	}

	private protected override void OnExecutionPerformed(ExecutionSpeed executionSpeed)
	{
		throw new NotImplementedException();
	}

	private protected override void OnGrowl(GrowlType growlType)
	{
		throw new NotImplementedException();
	}

	private protected override void OnHealthDepleted(DamageType damageType)
	{
		_npcStateMachine.ChangeState(typeof(Die));
	}

	private protected override void OnPlayerDeath()
	{
		_npcStateMachine.ChangeState(typeof(Idle));
	}

	private protected override void OnVulnerabilityTriggered()
	{
		throw new NotImplementedException();
	}

	private protected override void ProcessFrame(double delta)
	{
		TimeSinceLastAttack += delta;
	}

	public void PlayFireStreamAnimation(float streamAngle)
	{
		_fireStreamRoot.RotationDegrees = streamAngle;

		_fireStreamAnimationPlayer.Play("fire_stream");
	}
}
