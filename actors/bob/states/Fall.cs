using Epilogue.nodes;
using Godot;
using System.Threading.Tasks;

namespace Epilogue.actors.hestmor.states;
public partial class Fall : PlayerState
{
	public override void OnEnter()
	{
		AnimPlayer.Play("fall");
		Actor.CanChangeFacingDirection = true;
	}

	public override void PhysicsUpdate(double delta)
	{
		Actor.Velocity = new Vector2(Actor.Velocity.X, Actor.Velocity.Y + (Gravity * (float) delta));
		Actor.MoveAndSlideWithRotation();

		if(Actor.IsOnFloor())
		{
			StateMachine.ChangeState("Idle");
		}
		else if(Actor.RayCasts["Head"].IsColliding() && !Actor.RayCasts["Ledge"].IsColliding())
		{
			StateMachine.ChangeState("GrabLedge");
		}
	}

	public override async Task OnLeaveAsync()
	{
		AudioPlayer.PlayGenericSfx("Land");
		AnimPlayer.Play("fall_land");

		await ToSignal(AnimPlayer, "animation_finished");
	}
}
