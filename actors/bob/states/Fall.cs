using Epilogue.nodes;
using Godot;
using System.Threading.Tasks;

namespace Epilogue.actors.hestmor.states;
/// <summary>
///		State that allows Hestmor to fall from high places
/// </summary>
public partial class Fall : PlayerState
{
	internal override void OnEnter()
	{
		AnimPlayer.Play("fall");
		Player.CanChangeFacingDirection = true;
	}

	internal override void PhysicsUpdate(double delta)
	{
		Player.Velocity = new Vector2(Player.Velocity.X, Player.Velocity.Y + (Gravity * (float) delta));
		Player.MoveAndSlideWithRotation();

		if(Player.IsOnFloor())
		{
			StateMachine.ChangeState("Idle");
		}
		else if(Player.RayCasts["Head"].IsColliding() && !Player.RayCasts["Ledge"].IsColliding())
		{
			StateMachine.ChangeState("GrabLedge");
		}
	}

	internal override async Task OnLeaveAsync()
	{
		AudioPlayer.PlayGenericSfx("Land");
		AnimPlayer.Play("fall_land");

		await ToSignal(AnimPlayer, "animation_finished");
	}
}
