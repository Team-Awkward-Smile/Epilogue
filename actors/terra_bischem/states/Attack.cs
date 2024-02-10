using Epilogue.Const;
using Epilogue.Nodes;
using Godot;
using static Godot.GodotObject;

namespace Epilogue.Actors.TerraBischem.States;
/// <inheritdoc/>
public partial class Attack : State
{
	private readonly YoyoTerraBischem _terraBischem;
	private readonly Player _player;
	private readonly float _attackRange;

	private Tween _tween;
	private Vector2 _originalEyePosition;

	/// <summary>
	///		State that allows a Yoyo Terra Bischem to launch it's eye towards the player
	/// </summary>
	/// <param name="stateMachine">The State Machine who owns this State</param>
	/// <param name="player">The player character</param>
	/// <param name="attackRange">The range of the attack</param>
	public Attack(StateMachine stateMachine, Player player, float attackRange) : base(stateMachine)
	{
		_terraBischem = (YoyoTerraBischem)stateMachine.Owner;
		_player = player;
		_attackRange = attackRange;
	}

	internal override void OnEnter(params object[] args)
	{
		var attackDirection = (_player.GlobalPosition - new Vector2(0f, Constants.PLAYER_HEIGHT / 2) - _terraBischem.Sprite.GlobalPosition).Normalized();
		var targetPosition =  attackDirection * _attackRange;
		var windUpPosition = attackDirection * -20f + _terraBischem.Sprite.Position;

		_originalEyePosition = _terraBischem.Sprite.GlobalPosition;
		_tween = StateMachine.CreateTween();

		_tween.TweenProperty(_terraBischem.Sprite, "position", windUpPosition, 0.5f);
		_tween.Connect(Tween.SignalName.Finished, Callable.From(() => LaunchEye(targetPosition)), (uint)ConnectFlags.OneShot);
	}

	private void LaunchEye(Vector2 targetPosition)
	{
		_tween = StateMachine.CreateTween();

		_tween.TweenProperty(_terraBischem.Sprite, "position", targetPosition, 0.5f);
		_tween.Connect(Tween.SignalName.Finished, Callable.From(ReturnEyeToSocket), (uint)ConnectFlags.OneShot);
	}

	private void ReturnEyeToSocket()
	{
		_tween = StateMachine.CreateTween();

		_tween.TweenProperty(_terraBischem.Sprite, "global_position", _originalEyePosition, 0.5f);
		_tween.Connect(Tween.SignalName.Finished, Callable.From(() => StateMachine.ChangeState(typeof(Combat))), (uint)ConnectFlags.OneShot);
	}
}
