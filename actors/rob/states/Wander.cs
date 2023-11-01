using Epilogue.nodes;

using Godot;
using System;

public partial class Wander : NpcState
{
	[Export] private float _wanderSpeed = 50f;

	internal override async void OnEnter(params object[] args)
	{
		Npc.WaitingForNavigationQuery = true;

		var rng = new RandomNumberGenerator();

		await Npc.UpdatePathToWander(new Vector2(rng.RandfRange(-5000f, 5000f), rng.RandfRange(-5000f, 5000f)));

		Npc.WaitingForNavigationQuery = false;

		AnimPlayer.PlayBackwards("walk");
	}

	internal override void PhysicsUpdate(double delta)
	{
		if(Npc.WaitingForNavigationQuery)
		{
			return;
		}

		if(Npc.IsPlayerReachable)
		{
			StateMachine.ChangeState("Move");

			return;
		}

		Npc.Velocity = Npc.WanderNavigationAgent2D.GetNextVelocity(Npc.GlobalPosition, _wanderSpeed);

		Npc.MoveAndSlideWithRotation();
	}
}
