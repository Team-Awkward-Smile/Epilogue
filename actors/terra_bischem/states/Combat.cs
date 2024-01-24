using Epilogue.Nodes;
using Godot;

namespace Epilogue.Actors.TerraBischem.States;
public partial class Combat : State
{
	private readonly YoyoTerraBischem _terraBischem;
	private readonly Player _player;
	private readonly float _attackCooldown;
	private readonly float _attackRange;

	private bool _tweenFinished = false;

	public Combat(StateMachine stateMachine, Player player, float attackCooldown, float attackRange) : base(stateMachine)
	{
		_terraBischem = (YoyoTerraBischem)stateMachine.Owner;
		_player = player;
		_attackCooldown = attackCooldown;
		_attackRange = attackRange;
	}

	internal override void OnEnter(params object[] args)
	{
		var eyePosition = _terraBischem.Eye.GlobalPosition;
		var playerPosition = _player.GlobalPosition;
		var rotation = Mathf.Atan2(playerPosition.Y - eyePosition.Y, playerPosition.X - eyePosition.Y);
		var tween = StateMachine.CreateTween();

		tween.TweenProperty(_terraBischem.Eye, "rotation", rotation, 0.5f);
		tween.Finished += () => _tweenFinished = true;
	}

	internal override void Update(double delta)
	{
		if (!_tweenFinished)
		{
			return;
		}

		_terraBischem.TimeSinceLastAttack += (float)delta;
		_terraBischem.Eye.LookAt(_player.GlobalPosition);

		if (_terraBischem.TimeSinceLastAttack >= _attackCooldown && _terraBischem.DistanceToPlayer <= _attackRange)
		{
			_terraBischem.TimeSinceLastAttack = 0f;

			StateMachine.ChangeState(typeof(Attack));
		}
	}
}
