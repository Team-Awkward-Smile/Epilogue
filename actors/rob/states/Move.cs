using Epilogue.global.enums;
using Epilogue.nodes;

public partial class Move : NpcState
{
	private bool _isIdle = false;

	internal override void OnEnter()
	{
		Npc.CanChangeFacingDirection = true;
		AnimPlayer.PlayBackwards("walk");
	}

	internal override void PhysicsUpdate(double delta)
	{
		// The Velocity of the NPC is set in it's AI script
		Npc.MoveAndSlideWithRotation();

		if(Npc.Velocity.X != 0)
		{
			Npc.SetFacingDirection(Npc.Velocity.X > 0 ? ActorFacingDirection.Right : ActorFacingDirection.Left);

			if(_isIdle)
			{
				AnimPlayer.PlayBackwards("walk");
				_isIdle = false;
			}
		}
		else
		{
			AnimPlayer.PlayBackwards("idle");
			_isIdle = true;
		}
	}
}
