using Epilogue.global.singletons;

namespace Epilogue.nodes;
/// <summary>
///		Node representing a State that a Player Character can assume when using a State Machine.
/// </summary>
public partial class PlayerState : State
{
	/// <summary>
	///		Reference to the player character
	/// </summary>
	private protected Player Player { get; private set; }

	/// <summary>
	///		Singleton responsible for triggering events related to the player character
	/// </summary>
	private protected PlayerEvents PlayerEvents { get; private set; }

	private protected override void AfterReady()
	{
		Player = (Player) Owner;
		PlayerEvents = GetNode<PlayerEvents>("/root/PlayerEvents");
	}
}
