using Godot;

namespace Epilogue.Nodes;
/// <summary>
///		Node to be used as the base for every HurtBox in the game
/// </summary>
[GlobalClass]
public partial class HurtBox : Area2D
{
	[Export] private double _iFrameDuration = 0.5;

	public bool CanRecoverFromDamage { get; set; } = true;

	private Actor _actor;
	private bool _invulnerable;
	private double _iTimer;

	public override void _Ready()
	{
		_actor = (Actor)Owner;

		AreaEntered += (Area2D area) =>
		{
			if (area is HitBox hitBox)
			{
				_actor.ReduceHealth(hitBox.Damage, hitBox.DamageType);
				_invulnerable = true;
				_iTimer = _iFrameDuration;

				SetDeferred(Area2D.PropertyName.Monitorable, false);
				SetDeferred(Area2D.PropertyName.Monitoring, false);
			}
		};
	}

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
		}
	}
}