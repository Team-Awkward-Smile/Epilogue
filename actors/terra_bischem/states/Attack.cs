using Epilogue.Nodes;
using Godot;
using static Godot.GodotObject;

namespace Epilogue.Actors.TerraBischem.States;
public partial class Attack : State
{
	private readonly YoyoTerraBischem _terraBischem;
	private readonly Player _player;
	private readonly float _attackRange;

	private Tween _tween;
	private Vector2 _originalEyePosition;

	public Attack(StateMachine stateMachine, Player player, float attackRange) : base(stateMachine)
	{
		_terraBischem = (YoyoTerraBischem)stateMachine.Owner;
		_player = player;
		_attackRange = attackRange;
	}

	internal override void OnEnter(params object[] args)
	{
		var targetPosition = _terraBischem.Eye.GlobalPosition + ((_player.GlobalPosition - new Vector2(0f, Constants.Constants.PLAYER_HEIGHT / 2) - _terraBischem.Eye.GlobalPosition).Normalized() * _attackRange);

		_originalEyePosition = _terraBischem.Eye.GlobalPosition;
		_tween = StateMachine.CreateTween();

		_tween.TweenProperty(_terraBischem.Eye, "global_position", targetPosition, 0.5f);
		_tween.Connect(Tween.SignalName.Finished, Callable.From(ReturnEyeToSocket), (uint)ConnectFlags.OneShot);
	}

	private void ReturnEyeToSocket()
	{
		_tween = StateMachine.CreateTween();

		_tween.TweenProperty(_terraBischem.Eye, "global_position", _originalEyePosition, 0.5f);
		_tween.Connect(Tween.SignalName.Finished, Callable.From(() => StateMachine.ChangeState(typeof(Combat))), (uint)ConnectFlags.OneShot);
	}
}
