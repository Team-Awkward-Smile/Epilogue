using Epilogue.Global.Enums;
using Epilogue.Nodes;
using Godot;
using Godot.Collections;

namespace Epilogue.Actors.TerraBischem;
/// <summary>
///		Version of a Terra Bischem that can swing left and right
/// </summary>
public partial class RotatingAxe : Npc
{
	private Sprite2D _eyeSprite;
	private HitBox _hitBox;
	private AnimatedSprite2D _mouthSprite;

	/// <inheritdoc/>
	public override Dictionary<DamageType, float> DamageModifiers { get; set; } = new()
	{
		{ DamageType.Unarmed, -int.MaxValue },
		{ DamageType.Fire, +20 }
	};

	private protected override bool UseDefaultPathfinding => false;

	/// <inheritdoc/>
	public override void _Ready()
	{
		Sprite = GetNode<Sprite2D>("Sprite2D/Eye");
		HurtBox = GetNode<HurtBox>("Sprite2D/Eye/HurtBox");

		base._Ready();

		_eyeSprite = GetNode<Sprite2D>("Sprite2D/Eye");
		_hitBox = GetNode<HitBox>("%HitBox");
		_mouthSprite = GetNode<AnimatedSprite2D>("Sprite2D/MouthAnimatedSprite2D");

		_hitBox.ActorHit += () => _mouthSprite.Play();

		AnimationPlayer.Play("swing");
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

	private protected override void OnVulnerabilityTriggered()
	{
		return;
	}

	private protected override void ProcessFrame(double delta)
	{
		_eyeSprite.LookAt(Player.GlobalPosition);
	}
}
