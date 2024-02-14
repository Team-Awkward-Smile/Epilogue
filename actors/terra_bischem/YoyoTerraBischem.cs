using Epilogue.Actors.TerraBischem.States;
using Epilogue.Global.Enums;
using Epilogue.Nodes;
using Godot;
using Godot.Collections;

namespace Epilogue.Actors.TerraBischem;
/// <summary>
///		Version of a Terra Bischem that launches it's eye like an yoyo towards the player
/// </summary>
public partial class YoyoTerraBischem : Npc
{
	/// <summary>
	///		Distance (in units) to the player
	/// </summary>
	public float DistanceToPlayer { get; set; }

	/// <summary>
	///		Time (in seconds) since the last attack was performed
	/// </summary>
	public float TimeSinceLastAttack { get; set; }

	private Line2D _line2D;
	private Vector2 _pointOffset;
	private HitBox _eyeHitBox;

	/// <inheritdoc/>
	public override Dictionary<DamageType, float> DamageModifiers { get; set; } = new()
	{
		{ DamageType.Fire, 2f },
		{ DamageType.Light, 0.5f }
	};

	private protected override bool UseDefaultPathfinding => false;

	/// <inheritdoc/>
	public override void _Ready()
	{
		Sprite = GetNode<Sprite2D>("Eye");
		BloodEmitter = GetNode<BloodEmitter>("Eye/BloodEmitter");
		HurtBox = GetNode<HurtBox>("Eye/HurtBox");

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
		_npcStateMachine.ChangeState(typeof(Idle), false);
	}

	private protected override void OnVulnerabilityTriggered()
	{
		return;
	}

	private protected override void ProcessFrame(double delta)
	{
		DistanceToPlayer = GlobalPosition.DistanceTo(Player.GlobalPosition);

		_line2D.SetPointPosition(1, Sprite.Position - _pointOffset);
	}
}
