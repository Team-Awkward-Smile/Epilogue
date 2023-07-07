using Godot;
using Epilogue.nodes;

namespace Epilogue.actors.hestmot.states;
public partial class Growl : StateComponent
{
	public override void OnEnter()
	{
		Actor.CanChangeFacingDirection = false;

		var animation = Actor.Health.CurrentHealth switch
		{
			1 => "growl_strong",
			2 => "growl_medium",
			_ => "growl_weak"
		};

		AnimPlayer.Play(animation);
		AnimPlayer.AnimationFinished += EndGrowl;
	}

	private void EndGrowl(StringName animName)
	{
		AnimPlayer.AnimationFinished -= EndGrowl;
		StateMachine.ChangeState("Idle");
	}
}

