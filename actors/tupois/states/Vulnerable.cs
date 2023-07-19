using Epilogue.nodes;
using Godot;

namespace Epilogue.actors.tupois;
public partial class Vulnerable : StateComponent
{
	[Export] private float StunnedTime { get; set; }

	private float _timer;

	public override void OnEnter()
	{
		Actor.Sprite.Modulate = new Color(1f, 0f, 0f);

		_timer = 0f;
	}

	public override void Update(double delta)
	{
		// KNOWN: 68 - The timer will always increase, even when the player has interacted with this Actor and is choosing a Glory Kill Speed
		_timer += (float) delta;

		if(_timer >= StunnedTime)
		{
			Actor.Health.ApplyHealth(1);
			StateMachine.ChangeState("Walk");
		}
	}

	public override void OnLeave()
	{
		Actor.Sprite.Modulate = new Color(1f, 1f, 1f);
	}
}
