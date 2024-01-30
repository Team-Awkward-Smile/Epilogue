using Epilogue.Actors.TerraBischem.States;
using Epilogue.Global.Enums;
using Epilogue.Nodes;
using Godot;
using Godot.Collections;
using System;

namespace Epilogue.Actors.TerraBischem;
public partial class YoyoTerraBischem : Npc
{
	public Sprite2D Eye { get; set; }

	public float DistanceToPlayer { get; set; }
	public float TimeSinceLastAttack { get; set; }

	private Line2D _line2D;
	private Vector2 _pointOffset;
	private HitBox _eyeHitBox;

	public override Dictionary<DamageType, float> DamageModifiers { get; set; } = new()
	{
		{ DamageType.Fire, 2f },
		{ DamageType.Light, 0.5f }
	};

	private protected override bool UseDefaultPathfinding => false;

	public override void _Ready()
	{
		Eye = GetNode<Sprite2D>("Eye");
		BloodEmitter = GetNode<BloodEmitter>("Eye/BloodEmitter");

		_line2D = GetNode<Line2D>("Line2D");
		_pointOffset = new Vector2(0f, _line2D.Position.Y);

		base._Ready();

		_eyeHitBox = GetNode<HitBox>("%EyeHitBox");

		_eyeHitBox.AreaEntered += (Area2D area) =>
		{
			if (area.Owner is Player)
			{
				_npcStateMachine.ChangeState(typeof(Laugh));
			}
		};
	}

	private protected override void OnDamageTaken(float damage, float currentHp, DamageType damageType)
	{
		return;
	}

	private protected override void OnExecutionPerformed(ExecutionSpeed executionSpeed)
	{
		return;
	}

	private protected override void OnGrowl(GrowlType growlType)
	{
		return;
	}

	private protected override void OnHealthDepleted(DamageType damageType)
	{
		QueueFree();
	}

	private protected override void OnPlayerDeath()
	{
		return;
	}

	private protected override void OnVulnerabilityRecovered()
	{
		return;
	}

	private protected override void OnVulnerabilityTriggered()
	{
		return;
	}

	private protected override void ProcessFrame(double delta)
	{
		DistanceToPlayer = GlobalPosition.DistanceTo(Player.GlobalPosition);

		_line2D.SetPointPosition(1, Eye.Position - _pointOffset);
	}
}
