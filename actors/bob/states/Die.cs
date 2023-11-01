using Epilogue.nodes;

using Godot;

namespace Epilogue.actors.hestmor.states;
/// <summary>
///		State that makes Hestmor die and trigger the approprate events
/// </summary>
public partial class Die : PlayerState
{
	internal override void OnEnter(params object[] args)
	{
		Player.HurtBox.SetDeferred("monitorable", false);
		Player.HurtBox.SetDeferred("monitoring", false);
		Player.CanChangeFacingDirection = false;

		AnimPlayer.Play("Combat/die");
		AnimPlayer.AnimationFinished += (StringName animationName) =>
		{
			PlayerEvents.EmitGlobalSignal("PlayerDied");
		};
	}
}
