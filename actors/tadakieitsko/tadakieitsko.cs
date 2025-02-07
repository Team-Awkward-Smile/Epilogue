using Epilogue.Actors.TadakiEiTsko.States;
using Epilogue.Extensions;
using Epilogue.Global.Enums;
using Epilogue.Nodes;
using Godot;
using System.Collections.Generic;

namespace Epilogue.Actors.TadakiEiTsko
{
	/// <summary>
	/// AI logic for the enemy Tadaki Ei' Tsko (TET).
	/// </summary>
	public partial class TadakiEiTsko : Npc
	{
		// Damage modifiers for different attack types
		public override Dictionary<DamageType, float> DamageModifiers { get; set; } = new()
		{
			{ DamageType.GunThrow, 8f },
			{ DamageType.Unarmed, 5f },
			{ DamageType.Dung, 3f }
		};

		[Export] private float _detectionRange = 150f;
		[Export] private PackedScene _projectileScene;
		[Export] private Node2D _projectileSpawnPoint;

		private readonly RandomNumberGenerator _rng = new();
		private Timer _attackCooldownTimer;
		private bool _isInWater;
		private bool _isOnLand;
		private bool _isOnString;

		public override void _Ready()
		{
			base._Ready();

			_attackCooldownTimer = GetNode<Timer>("AttackCooldownTimer");
			_attackCooldownTimer.Timeout += ResetToIdle;

			// Starts in passive state
			_npcStateMachine.ChangeState(typeof(Idle));
		}

		/// <summary>
		/// Called when the player is detected.
		/// </summary>
		private protected override void OnPlayerDetected()
		{
			if (_isOnString)
			{
				DropFromString();
			}
			else if (_isInWater)
			{
				JumpAndThrowSpear();
			}
			else if (_isOnLand)
			{
				_npcStateMachine.ChangeState(typeof(MeleeAttack));
			}
		}

		/// <summary>
		/// Called when the enemy takes damage.
		/// </summary>
		private protected override void OnDamageTaken(float damage, float currentHp, DamageType damageType)
		{
			_npcStateMachine.ChangeState(typeof(TakeDamage), damageType);
		}

		/// <summary>
		/// Called when the enemy's health reaches zero.
		/// </summary>
		private protected override void OnHealthDepleted(DamageType damageType)
		{
			_npcStateMachine.ChangeState(typeof(Die));
		}

		/// <summary>
		/// Drops TET from an organic string.
		/// </summary>
		private void DropFromString()
		{
			_isOnString = false;
			_isOnLand = true;
			_npcStateMachine.ChangeState(typeof(Idle));
		}

		/// <summary>
		/// Makes TET jump out of the water and throw a spear.
		/// </summary>
		private void JumpAndThrowSpear()
		{
			_npcStateMachine.ChangeState(typeof(JumpAttack));
		}

		/// <summary>
		/// Throws a projectile toward the player.
		/// </summary>
		public void ThrowProjectile()
		{
			if (_projectileScene == null || _projectileSpawnPoint == null) return;

			var projectile = (TetProjectile)_projectileScene.Instantiate();
			GetParent().AddChild(projectile);
			projectile.GlobalPosition = _projectileSpawnPoint.GlobalPosition;
			projectile.SetDirection((Player.GlobalPosition - GlobalPosition).Normalized());
		}

		/// <summary>
		/// Resets the enemy to the idle state.
		/// </summary>
		private void ResetToIdle()
		{
			_npcStateMachine.ChangeState(typeof(Idle));
		}
	}
}
