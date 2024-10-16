using Epilogue.Nodes;
using Godot;
using System.Linq;
using System.Threading.Tasks;

namespace Epilogue.Actors.Hestmor.States;
/// <inheritdoc/>
public partial class Sleep : State
{
	private readonly Player _player;

	/// <summary>
	///     State that makes Hestmor go to sleep when no inputs are detected for a while
	/// </summary>
	/// <param name="stateMachine"></param>
	public Sleep(StateMachine stateMachine) : base(stateMachine)
	{
		_player = (Player)stateMachine.Owner;

		SpriteSheetId = (int)Enums.SpriteSheetId.Bob;
	}

	internal override void OnStateMachineActivation()
	{
		AnimPlayer.AnimationFinished += (StringName animationName) =>
		{
			if (!Active || animationName != "Sleep/sleep_start")
			{
				return;
			}

			AnimPlayer.Play("Sleep/sleep_loop");
		};
	}

	internal override void OnInput(InputEvent @event)
	{
		var actions = new string[] { "move_left", "move_right", "jump", "slide", "melee", "interact", "shoot" };

		foreach (var a in actions.Where(action => @event.IsActionPressed(action)))
		{
			StateMachine.ChangeState(typeof(Idle));

			return;
		}
	}

	internal override void OnEnter(params object[] args)
	{
		_player.CanChangeFacingDirection = false;
		_player.CanInteract = false;

		AnimPlayer.Play("Sleep/sleep_start");
	}

	internal override async Task OnLeave()
	{
		AnimPlayer.Play("Sleep/sleep_end");

		await StateMachine.ToSignal(AnimPlayer, AnimationMixer.SignalName.AnimationFinished);

		_player.CanChangeFacingDirection = true;
		_player.CanInteract = true;
	}
}
