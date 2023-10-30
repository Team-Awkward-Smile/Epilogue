using Epilogue.actors.hestmor.enums;
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
	private StateType _jumpType;
	private string _animation;
	private JumpData _jumpData;

	internal override void OnEnter(params object[] args)
	{
		if(args.Length > 1)
		{
			_jumpData = (JumpData) args[1];
		}
		else
		{
			_jumpData = new();
		}

		_jumpType = (StateType) args[0];
		_animation = _jumpType switch 
		{
			StateType.VerticalJump => "vertical",
			_ => "long"
		};

		_canGrabLedge = false;
		_playLandingAnimation = true;

		AnimPlayer.Play($"Jump/{_animation}_jump_down");
		Player.CanChangeFacingDirection = true;

		GetTree().CreateTimer(0.1f).Timeout += () => _canGrabLedge = true;
	}

	internal override void PhysicsUpdate(double delta)
	{
		_jumpData.MaxSpeed = new(Mathf.Max(_jumpData.MaxSpeed.X, Player.Velocity.X), Mathf.Min(_jumpData.MaxSpeed.Y, Player.Velocity.Y));
		_jumpData.Duration += (float) delta;

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

		var verticalSpeedModifer = Player.Conditions.HasFlag(Conditions.Sinking) ? Mathf.Min(Player.Velocity.Y, 0f) : Player.Velocity.Y;

		Player.Velocity = new Vector2(Player.Velocity.X, verticalSpeedModifer + (Player.Gravity * (float) delta));
		Player.MoveAndSlideWithRotation();

		if(Player.IsOnFloor() || Player.Conditions.HasFlag(Conditions.Sinking))
		{
			_jumpData.EndPosition = Player.Position;

			GD.PrintRich($"\n[b]Jump Data[/b]:\n" +
			$"- Distance: {_jumpData.Distance}\n" +
			$"- Max Speed: {_jumpData.MaxSpeed}\n" +
			$"- Duration: {_jumpData.Duration} s\n" +
			$"- Tiles: {_jumpData.Tiles}");

			StateMachine.ChangeState("Idle");
		}
	}

	internal override async Task OnLeaveAsync()
	{
		if(!_playLandingAnimation)
		{
			return;
		}

		Player.Velocity = Vector2.Zero;
		AudioPlayer.PlayGenericSfx("Land");
		AnimPlayer.Play($"Jump/{_animation}_jump_land");

		await ToSignal(AnimPlayer, "animation_finished");
	}
}
