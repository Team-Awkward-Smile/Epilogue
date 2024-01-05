using Epilogue.Extensions;
using Epilogue.Nodes;
using Godot;

namespace Epilogue.Actors.rob.states;
/// <inheritdoc/>
public partial class Die : State
{
	private readonly Rob _rob;

	/// <summary>
	/// 	State that allows Rob to die when his HP reaches 0
	/// </summary>
	/// <param name="stateMachine">The State Machine who owns this State</param>
	public Die(StateMachine stateMachine) : base(stateMachine)
	{
		_rob = (Rob) stateMachine.Owner;
	}

	internal override void OnEnter(params object[] args)
	{
		_rob.Sprite.SetShaderMaterialParameter("iframeActive", false);
		
		AnimPlayer.PlayBackwards("Combat/die");
		AnimPlayer.AnimationFinished += (StringName animName) => _rob.QueueFree();
	}
}
