using Epilogue.nodes;

namespace Epilogue.actors.hestmor.states;
/// <summary>
///		State that makes Hestmor die and trigger the approprate events
/// </summary>
public partial class Die : PlayerState
{
	internal override void OnEnter()
	{
		Player.CanChangeFacingDirection = false;

		AnimPlayer.Play("die");
	}
}
