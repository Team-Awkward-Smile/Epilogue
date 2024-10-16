using Epilogue.Nodes;
using Godot;
using static Godot.GodotObject;

namespace Epilogue.Actors.Hestmor.States;
/// <inheritdoc/>
public partial class TakeDamage : State
{
	private readonly Player _player;

	/// <summary>
	/// 	State that allows Hestmor to react to damage taken
	/// </summary>
	/// <param name="stateMachine">The State Machine who owns this State</param>
	public TakeDamage(StateMachine stateMachine) : base(stateMachine)
	{
		_player = (Player)stateMachine.Owner;

		SpriteSheetId = (int)Enums.SpriteSheetId.Bob;
	}

	internal override void OnStateMachineActivation()
	{
		AnimPlayer.AnimationFinished += (StringName animationName) =>
		{
			if (!Active || animationName != "Combat/take_damage")
			{
				return;
			}

			StateMachine.ChangeState(typeof(Idle));
		};
	}

	internal override void OnEnter(params object[] args)
	{
		_player.CanChangeFacingDirection = false;

		AnimPlayer.Play("Combat/take_damage");
	}
}
