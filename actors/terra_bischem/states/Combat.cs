using Epilogue.Nodes;
using Godot;

namespace Epilogue.Actors.TerraBischem.States;
/// <inheritdoc/>
public partial class Combat : State
{
	private readonly YoyoTerraBischem _terraBischem;
	private readonly Player _player;
	private readonly float _attackCooldown;
	private readonly float _attackRange;

	private bool _tweenFinished = false;

	/// <summary>
	///		State that allows a Yoyo Terra Bischem to prepare an attack when the player is close enough
	/// </summary>
	/// <param name="stateMachine">The State Machine who owns this State</param>
	/// <param name="player">The player character</param>
	/// <param name="attackCooldown">The cooldown (in seconds) between attacks</param>
	/// <param name="attackRange">The range (in units) the player must be for an attack to be performed</param>
	public Combat(StateMachine stateMachine, Player player, float attackCooldown, float attackRange) : base(stateMachine)
	{
		_terraBischem = (YoyoTerraBischem)stateMachine.Owner;
		_player = player;
		_attackCooldown = attackCooldown;
		_attackRange = attackRange;
	}

	internal override void OnEnter(params object[] args)
	{
		var eyePosition = _terraBischem.Sprite.GlobalPosition;
		var playerPosition = _player.GlobalPosition;
		var rotation = Mathf.Atan2(playerPosition.Y - eyePosition.Y, playerPosition.X - eyePosition.X);
		var tween = StateMachine.CreateTween();

		tween.TweenProperty(_terraBischem.Sprite, "rotation", rotation, 0.5f);
		tween.Finished += () => _tweenFinished = true;
	}

	internal override void Update(double delta)
	{
		if (!_tweenFinished)
		{
			return;
		}

		_terraBischem.TimeSinceLastAttack += (float)delta;
		_terraBischem.Sprite.LookAt(_player.GlobalPosition);

		if (_terraBischem.TimeSinceLastAttack >= _attackCooldown && _terraBischem.DistanceToPlayer <= _attackRange)
		{
			_terraBischem.TimeSinceLastAttack = 0f;

			StateMachine.ChangeState(typeof(Attack));
		}
	}
}
