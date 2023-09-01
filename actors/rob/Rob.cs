using Epilogue.nodes;

namespace Epilogue.actors.rob;
/// <summary>
///		Script controlling Rob's AI
/// </summary>
public partial class Rob : Npc
{
	private protected override void OnVulnerabilityTriggered()
	{
		StateMachine.ChangeState("Stun");
	}

	private protected override void OnVulnerabilityExpired()
	{
		StateMachine.ChangeState("Walk");
	}

	private protected override void OnHealthDepleted()
	{
		StateMachine.ChangeState("Die");
	}
}
