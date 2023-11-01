using Epilogue.nodes;

using Godot;

public partial class Flee : NpcState
{
	internal override void OnEnter(params object[] args)
	{
		GetTree().CreateTimer(Npc.CustomVariables["FleeDuration"].AsSingle()).Timeout += () => StateMachine.ChangeState("Move");
		AnimPlayer.Play("walk", -1, 2, true);
	}

	internal override void PhysicsUpdate(double delta)
	{
		var fleeSpeed = Npc.CustomVariables["FleeSpeed"].AsSingle();

		var velocity = Npc.PlayerNavigationAgent2D.GetNextVelocity(Npc.GlobalPosition, fleeSpeed) * -1;

		Npc.Velocity = new Vector2(velocity.X, velocity.Y + Gravity * (float) delta);
		Npc.MoveAndSlideWithRotation();
	}
}
