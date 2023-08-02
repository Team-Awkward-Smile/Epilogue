using Godot;
using Epilogue.nodes;

namespace Epilogue.actors.hestmot.states;
/// <summary>
///		State that allows Hestmor to growl and taunt nearby enemies
/// </summary>
public partial class Growl : PlayerState
{
	internal override void OnEnter()
	{
		Player.CanChangeFacingDirection = false;

		var animation = Player.Health.CurrentHealth switch
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

