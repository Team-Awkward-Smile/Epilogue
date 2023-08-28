using Epilogue.constants;
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
	private bool _canGrabLedge;

	internal override void OnEnter()
	{
		_canGrabLedge = false;
		_playLandingAnimation = true;

		AnimPlayer.Play("fall");
		Player.CanChangeFacingDirection = true;

		GetTree().CreateTimer(0.1f).Timeout += () => _canGrabLedge = true;
	}

	internal override void PhysicsUpdate(double delta)
	{
		if(_canGrabLedge && Player.IsOnWall() && Player.SweepForLedge(out var ledgePosition))
		{
			var offset = Player.RayCasts["Head"].GlobalPosition.Y - ledgePosition.Y;

			_playLandingAnimation = false;

			if(offset < -20)
			{
				Player.Position = new Vector2(Player.Position.X, ledgePosition.Y + Constants.MAP_TILE_SIZE);
				StateMachine.ChangeState("Vault");
			}
			else
			{
				Player.Position -= new Vector2(0f, offset);
				StateMachine.ChangeState("GrabLedge");
			}

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
