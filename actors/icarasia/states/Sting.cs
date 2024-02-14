using Epilogue.Actors.Icarasia.Enums;
using Epilogue.Nodes;
using Godot;

namespace Epilogue.Actors.Icarasia.States;
/// <inheritdoc/>
public class Sting : State
{
	private readonly Icarasia _icarasia;
	private readonly Node2D _stingerPivot;

	/// <summary>
	///		State used by the Icarasia to perform a sting attack
	/// </summary>
	/// <param name="stateMachine">State Machine who owns this State</param>
	public Sting(StateMachine stateMachine) : base(stateMachine)
	{
		_icarasia = (Icarasia)stateMachine.Owner;
		_stingerPivot = stateMachine.GetNode<Node2D>("../FlipRoot/StingerPivot");
	}

	internal override void OnEnter(params object[] args)
	{
		var direction = (StingDirection)args[0];

		_stingerPivot.RotationDegrees = direction switch
		{
			StingDirection.Forward => 0f,
			StingDirection.Down => 90f,
			_ => 0f
		};

		AnimPlayer.Play("sting");

		_icarasia.AttackTimer = 0f;
		_icarasia.StingDirection = null;

		var rng = new RandomNumberGenerator();

		StateMachine.GetTree().CreateTimer(rng.RandfRange(1.5f, 3.5f)).Timeout += () => StateMachine.ChangeState(typeof(Move));
	}
}