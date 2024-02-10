using Godot;

namespace Epilogue.Nodes;
/// <summary>
///		Node to be used as the base for every HurtBox in the game
/// </summary>
[GlobalClass]
public partial class HurtBox : Area2D
{
	/// <summary>
	///		Signal emitted whenever this HurtBox is temporarily disabled after receiving damage
	/// </summary>
	[Signal] public delegate void HurtBoxDisabledEventHandler();

	/// <summary>
	///		Signal emitted whenever this HurtBox is reenabled after receiving damage
	/// </summary>
	[Signal] public delegate void HurtBoxEnabledEventHandler();

	[Export] private double _iFrameDuration = 0.5;

	/// <summary>
	///		Defines if this HurtBox will be reenabled after receiving damage.
	///		Setting this to false will prevent the <see cref="HurtBoxDisabled"/> signal from being emitted
	/// </summary>
	public bool CanRecoverFromDamage { get; set; } = true;

	private Actor _actor;
	private bool _invulnerable;
	private double _iTimer;

	/// <inheritdoc/>
	public override void _Ready()
	{
		_actor = (Actor)Owner;
	}

	/// <inheritdoc/>
	public override void _PhysicsProcess(double delta)
	{
		if (!CanRecoverFromDamage)
		{
			return;
		}

		if (_invulnerable && (_iTimer -= delta) <= 0)
		{
			_invulnerable = false;

			SetDeferred(Area2D.PropertyName.Monitorable, true);
			SetDeferred(Area2D.PropertyName.Monitoring, true);

			EmitSignal(SignalName.HurtBoxEnabled);
		}
	}

	/// <summary>
	///		Method called by a HitBox hitting this HurtBox.
	///		Reduces the Actor's HP and makes them blink
	/// </summary>
	/// <param name="hitBox">The HitBox that hit this HurtBox</param>
	public void OnHit(HitBox hitBox)
	{
		_actor.ReduceHealth(hitBox.Damage, hitBox.DamageType);
		_invulnerable = true;
		_iTimer = _iFrameDuration;

		SetDeferred(Area2D.PropertyName.Monitorable, false);
		SetDeferred(Area2D.PropertyName.Monitoring, false);

		if (CanRecoverFromDamage)
		{
			EmitSignal(SignalName.HurtBoxDisabled);
		}
	}
}