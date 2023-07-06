using Epilogue.extensions;
using Epilogue.nodes;
using Godot;
using System.Threading.Tasks;

namespace Epilogue.actors.hestmor.states;
public partial class Fall : StateComponent
{
	public override void OnEnter()
	{
		AnimPlayer.Play("Bob/Jumping_descend");
	}

	public override void PhysicsUpdate(double delta)
	{
		Actor.Velocity = new Vector2(Actor.Velocity.X, Actor.Velocity.Y + (Gravity * (float) delta));
		Actor.MoveAndSlideWithRotation();

		if(Actor.IsOnFloor())
		{
			StateMachine.ChangeState("Idle");
		}
		else if(Actor.IsOnWall() && !Actor.IsHeadRayCastColliding())
		{
			//StateMachine.ChangeState("GrabLedge");
		}
	}

	public override async Task OnLeaveAsync()
	{
		AudioPlayer.PlaySfx("Land");
		AnimPlayer.Play("Bob/Jumping_land");

		await ToSignal(AnimPlayer, "animation_finished");
	}
}
