using Epilogue.nodes;
using Godot;
using System.Threading.Tasks;

namespace Epilogue.actors.hestmor.states;
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
        _player = (Player) stateMachine.Owner;
    }

	internal override void OnInput(InputEvent @event)
	{
        var actions = new string[] { "move_left", "move_right", "jump", "slide", "melee", "interact", "shoot" };

        foreach(var a in actions)
        {
            if(@event.IsActionPressed(a))
            {
                StateMachine.ChangeState(typeof(Idle));
            }
        }
	}

	internal override void OnEnter(params object[ ] args)
	{
		_player.CanChangeFacingDirection = false;

        AnimPlayer.Play("Sleep/sleep_start");
        AnimPlayer.AnimationFinished += StartSleepLoop;
	}

    private void StartSleepLoop(StringName animName)
    {
        AnimPlayer.AnimationFinished -= StartSleepLoop;

        AnimPlayer.Play("Sleep/sleep_loop");
    }

	internal override async Task OnLeave()
	{
		AnimPlayer.Play("Sleep/sleep_end");

        await StateMachine.ToSignal(AnimPlayer, "animation_finished");
	}
}
