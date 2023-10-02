using Epilogue.actors.hestmor.enums;
using Epilogue.constants;
using Epilogue.nodes;
using Godot;
using System.Security.AccessControl;
using System.Threading.Tasks;

namespace Epilogue.actors.hestmor.states;
/// <summary>
///		State that allows Hestmor to fall from high places
/// </summary>
public partial class Fall : PlayerState
{
	private bool _playLandingAnimation = true;
	private bool _canGrabLedge;
	private StateType _jumpType;

	internal override void OnEnter(params object[] args)
	{
		_jumpType = args.Length > 0 ? (StateType) args[0] : StateType.VerticalJump;

		var animation = _jumpType switch
		{
			StateType.CrawlJump => "crawl_fall",
			_ => "fall"
		};

		_canGrabLedge = false;
		_playLandingAnimation = true;

		AnimPlayer.Play(animation);
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
				StateMachine.ChangeState("GrabLedge", StateType.FallGrab);
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

		var animation = _jumpType switch
		{
			StateType.CrawlJump => "crawl_fall_land",
			_ => "fall_land"
		};

		AudioPlayer.PlayGenericSfx("Land");
		AnimPlayer.Play(animation);

		await ToSignal(AnimPlayer, "animation_finished");
	}
}
