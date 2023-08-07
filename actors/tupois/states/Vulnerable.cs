using Epilogue.nodes;
using Godot;

namespace Epilogue.actors.tupois;
/// <summary>
///		State that makes the Tupois Vulnerable to Executions
/// </summary>
public partial class Vulnerable : NpcState
{
	[Export] private float StunnedTime { get; set; }

	private float _timer;

	internal override void OnEnter()
	{
		Npc.Sprite.Modulate = new Color(1f, 0f, 0f);

		_timer = 0f;
	}

	internal override void Update(double delta)
	{
		// KNOWN: 68 - The timer will always increase, even when the player has interacted with this Actor and is choosing a Glory Kill Speed
		_timer += (float) delta;

		if(_timer >= StunnedTime)
		{
			Npc.ApplyHealth(1);
			StateMachine.ChangeState("Walk");
		}
	}

	internal override void OnLeave()
	{
		Npc.Sprite.Modulate = new Color(1f, 1f, 1f);
	}
}
