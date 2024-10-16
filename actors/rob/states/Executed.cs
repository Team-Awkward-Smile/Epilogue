using Epilogue.Extensions;
using Epilogue.Nodes;
using Godot;

namespace Epilogue.Actors.rob.states;
/// <inheritdoc/>
public partial class Executed : State
{
	private readonly Rob _rob;

	/// <summary>
	/// 	State that controls what happens with Rob when he is executed by the player
	/// </summary>
	/// <param name="stateMachine">The State Machine who owns this State</param>
	public Executed(StateMachine stateMachine) : base(stateMachine)
	{
		_rob = (Rob) stateMachine.Owner;
	}

	internal override void OnEnter(params object[] args)
	{
		_rob.Sprite.SetShaderMaterialParameter("iframeActive", false);

		AnimPlayer.PlayBackwards("Combat/execution");
		AnimPlayer.AnimationFinished += (StringName animName) =>
		{
			StateMachine.GetTree().CreateTimer(2f).Timeout += () =>
			{
				_rob.QueueFree();
			};
		};
	}
}
