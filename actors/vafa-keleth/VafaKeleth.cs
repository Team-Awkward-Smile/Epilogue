using Epilogue.Actors.VafaKeleth.States;
using Epilogue.Extensions;
using Epilogue.Global.Enums;
using Epilogue.Nodes;
using Godot;
using Godot.Collections;

namespace Epilogue.Actors.VafaKeleth;
/// <summary>
///		AI implementation of Vafa'Keleth
/// </summary>
public partial class VafaKeleth : Npc
{
	/// <inheritdoc/>
	public override Dictionary<DamageType, float> DamageModifiers { get; set; } = new()
	{
		{ DamageType.GunThrow, 2f },
		{ DamageType.Water, 2f },
		{ DamageType.Spread, 0.5f }
	};

	[Export] private bool _slideWhenShot = true;
	[Export(PropertyHint.Range, "0,1")] private float _slidePercentageChance = 0.3f;

	private protected override bool UseDefaultPathfinding => true;

	/// <summary>
	///		Time (in seconds) since the last attack was performed
	/// </summary>
	public double TimeSinceLastAttack { get; set; }

	/// <summary>
	///		Angles (in degrees) used to check for valid attacks
	/// </summary>
	public int[] AttackAngles { get; private set; } = new int[] { 0, 90 };

	/// <summary>
	///		Determines if this specific Vafa'Keleth can attempt to slide when being shot.
	///		If <c>true</c>, the chance to perform a slide is determined by <see cref="_slidePercentageChance"/>
	/// </summary>
	public bool CanAttemptSlide { get; set; } = true;

	private AnimationPlayer _fireStreamAnimationPlayer;
	private Node2D _fireStreamRoot;
	private RandomNumberGenerator _rng = new();
	private Sprite2D _fireSprite;
	private AttackType _currentAttackType = AttackType.Regular;
	private Timer _rangeResetTimer;

	/// <inheritdoc/>
	public override void _Ready()
	{
		base._Ready();

		_fireStreamAnimationPlayer = GetNode<AnimationPlayer>("%FireStreamAnimationPlayer");
		_fireStreamRoot = GetNode<Node2D>("%FireStreamRoot");
		_fireSprite = GetNode<Sprite2D>("%FireStream");
		_rangeResetTimer = GetNode<Timer>("RangeResetTimer");

		_rangeResetTimer.Timeout += () => SetAttackRange(_currentAttackType);
	}

	private protected override void OnDamageTaken(float damage, float currentHp, DamageType damageType)
	{
		_currentAttackType = AttackType.Punch;

		SetAttackRange(AttackType.Punch);

		if (IsOnFloor())
		{
			_npcStateMachine.ChangeState(typeof(TakeDamage), damageType);
		}
	}

	private protected override void OnExecutionPerformed(ExecutionSpeed executionSpeed)
	{
		_npcStateMachine.ChangeState(typeof(Execution), executionSpeed);
	}

	private protected override void OnGrowl(GrowlType growlType)
	{
		switch (growlType)
		{
			case GrowlType.Weak:
				_npcStateMachine.ChangeState(typeof(Stun), 0.6f);
				break;

			case GrowlType.Medium:
				_npcStateMachine.ChangeState(typeof(WalkBack), 1f);
				break;

			case GrowlType.Strong:
				_npcStateMachine.ChangeState(typeof(WalkBack), 0.6f);
				_rangeResetTimer.Start();

				SetAttackRange(AttackType.Punch);

				break;
		}
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
		Sprite.SetShaderMaterialParameter("vulnerabilityActive", true);

		_npcStateMachine.ChangeState(typeof(Vulnerable), 5.0);
	}

	private protected override void ProcessFrame(double delta)
	{
		TimeSinceLastAttack += delta;
	}

	private protected override void OnProjectileNotification()
	{
		if (_slideWhenShot && CanAttemptSlide && _rng.Randf() <= _slidePercentageChance)
		{
			_npcStateMachine.ChangeState(typeof(Slide));
		}
	}

	private protected override void OnDesperationTriggered()
	{
		_currentAttackType = AttackType.Desperation;

		SetAttackRange(AttackType.Desperation);

		_npcStateMachine.ChangeState(typeof(Combat));
	}

	/// <summary>
	///		Plays the animation of the Spit Fire projectile, automatically updating it's Sprite, size, HitBox, etc.
	/// </summary>
	public void PlayFireStreamAnimation()
	{
		_fireStreamRoot.RotationDegrees = ShapeCasts["Attack"].RotationDegrees;

		_fireStreamAnimationPlayer.Play("fire_stream");
	}

	private void SetAttackRange(AttackType attackType)
	{
		AttackAngles = attackType switch
		{
			AttackType.Punch => new int[] { 0 },
			AttackType.Regular => new int[] { 0, 90 },
			_ => new int[] { -90, -45, 0, 45, 90 }
		};

		PlayerNavigationAgent2D.TargetDesiredDistance = attackType == AttackType.Punch ? 25f : 10f;
		PlayerNavigationAgent2D.TargetPosition = Player.GlobalPosition;

		ShapeCasts["Attack"].TargetPosition = attackType switch 
		{
			AttackType.Punch => new Vector2(20f, 0f),
			AttackType.Regular => new Vector2(115f, 0f),
			_ => new Vector2(250f, 0f)
		};

		_fireSprite.Scale = attackType == AttackType.Desperation ? new Vector2(3f, 1f) : new Vector2(1f, 1f);
	}

	private enum AttackType
	{
		Punch,
		Regular,
		Desperation
	}
}
