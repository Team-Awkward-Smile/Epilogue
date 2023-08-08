using Epilogue.nodes;
using Godot;
using System.Threading.Tasks;

namespace Epilogue.actors.hestmor.states;
/// <summary>
///		State that allows Hestmor to fall from high places
/// </summary>
public partial class Fall : PlayerState
{
	private bool _playLandingAnimation = true;

	internal override void OnEnter()
	{
		AnimPlayer.Play("fall");
		Player.CanChangeFacingDirection = true;
	}

	internal override void PhysicsUpdate(double delta)
	{
		if(Player.RayCasts["Head"].IsColliding() && !Player.RayCasts["Ledge"].IsColliding())
		{
			_playLandingAnimation = false;

			StateMachine.ChangeState("GrabLedge");
			return;
		}

		Player.Velocity = new Vector2(Player.Velocity.X, Player.Velocity.Y + (Gravity * (float) delta));
		Player.MoveAndSlideWithRotation();

		if(Player.IsOnFloor())
		{
			StateMachine.ChangeState("Idle");
		}
	}

	internal override async Task OnLeaveAsync()
	{
		if(!_playLandingAnimation)
		{
			return;
		}

		AudioPlayer.PlayGenericSfx("Land");
		AnimPlayer.Play("fall_land");

		await ToSignal(AnimPlayer, "animation_finished");
	}
}
