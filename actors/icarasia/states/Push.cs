using Epilogue.Global.Enums;
using Epilogue.Nodes;
using Godot;

namespace Epilogue.Actors.Icarasia.States;
/// <inheritdoc/>
public partial class Push : State
{
	private readonly Icarasia _icarasia;

	/// <summary>
	///		State used by the Icarasia when being pushed by the player
	/// </summary>
	/// <param name="stateMachine">State Machine who owns this State</param>
	public Push(StateMachine stateMachine) : base(stateMachine)
	{
		_icarasia = (Icarasia)stateMachine.Owner;
	}

	internal override void OnEnter(params object[] args)
	{
		_icarasia.Velocity = new Vector2(50f * (_icarasia.FacingDirection == ActorFacingDirection.Left ? 1 : -1), -10f);
		_icarasia.GetTree().CreateTimer((float)args[0]).Timeout += () => StateMachine.ChangeState(typeof(Move));
	}

	internal override void PhysicsUpdate(double delta)
	{
		_ = _icarasia.MoveAndSlide();
	}
}
