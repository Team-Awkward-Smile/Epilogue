using Epilogue.nodes;
using Godot;
using System;
using System.Threading.Tasks;

public partial class Sleep : PlayerState
{
	internal override void OnInput(InputEvent @event)
	{
        var actions = new string[] { "move_left", "move_right", "jump", "slide", "melee", "interact", "shoot" };

        foreach(var a in actions)
        {
            if(@event.IsActionPressed(a))
            {
                StateMachine.ChangeState("Idle");
            }
        }
	}

	internal override void OnEnter(params object[ ] args)
	{
		Player.CanChangeFacingDirection = false;

        AnimPlayer.Play("Sleep/sleep_start");
        AnimPlayer.AnimationFinished += StartSleepLoop;
	}

    private void StartSleepLoop(StringName animName)
    {
        AnimPlayer.AnimationFinished -= StartSleepLoop;

        AnimPlayer.Play("Sleep/sleep_loop");
    }

	internal override async Task OnLeaveAsync()
	{
		AnimPlayer.Play("Sleep/sleep_end");

        await ToSignal(AnimPlayer, "animation_finished");
	}
}
