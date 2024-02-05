using Epilogue.Global.Enums;
using Epilogue.Nodes;
using Godot;
using Godot.Collections;

namespace Epilogue.Actors.TerraBischem;
/// <summary>
///		A version of Terra Bischem in the form of a blade that swings around it's pivot points trying to hit the player
/// </summary>
public partial class BladeTerraBischem : Npc
{
	/// <inheritdoc/>
	public override Dictionary<DamageType, float> DamageModifiers { get; set; } = new()
	{
		{ DamageType.Fire, 2f },
		{ DamageType.Light, 0.5f }
	};

	private protected override bool UseDefaultPathfinding => false;

	private AnimatedSprite2D _animatedSprite2D;
	private CollisionShape2D _leftHitBox;
	private CollisionShape2D _rightHitBox;

	/// <inheritdoc/>
	public override void _Ready()
	{
		Sprite = GetNode<Sprite2D>("Sprite2D");

		base._Ready();

		_animatedSprite2D = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		_leftHitBox = GetNode<CollisionShape2D>("HitBoxLeft/CollisionShape2D");
		_rightHitBox = GetNode<CollisionShape2D>("HitBoxRight/CollisionShape2D");

		_animatedSprite2D.FrameChanged += () =>
		{
			_leftHitBox.Disabled = _animatedSprite2D.Frame is not (2 or 3 or 4);
			_rightHitBox.Disabled = _animatedSprite2D.Frame is not (8 or 9 or 10);
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

	private protected override void OnVulnerabilityTriggered()
	{
		return;
	}

	private protected override void ProcessFrame(double delta)
	{
		return;
	}
}
