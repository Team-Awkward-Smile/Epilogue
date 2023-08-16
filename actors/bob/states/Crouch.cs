using Epilogue.nodes;
using Godot;
using System.Threading.Tasks;

namespace Epilogue.actors.hestmor.states;
/// <summary>
///		State that allows Hestmor to crouch
/// </summary>
public partial class Crouch : PlayerState
{
	[Export] private float _cameraMovementDelay = 0.5f;
	[Export] private int _cameraMovementDistance = 50;

	private float _timer;
	private Tween _cameraMovementTween;
	private bool _isCameraMoving;
	private CameraAnchor _cameraAnchor;

	internal override void OnInput(InputEvent @event)
	{
		if(@event.IsActionReleased(CrouchInput))
		{
			_cameraMovementTween?.Stop();

			StateMachine.ChangeState("Idle");
		}
	}

	internal override void OnEnter()
	{
		Player.CanChangeFacingDirection = false;

		_timer = 0f;
		_isCameraMoving = false;
		_cameraAnchor = Player.GetNode<CameraAnchor>("CameraAnchor");

		AnimPlayer.Play("crouch");
	}

	internal override void Update(double delta)
	{
		_timer += (float) delta;

		if(_timer >= _cameraMovementDelay && !_isCameraMoving)
		{
			_isCameraMoving = true;
			_cameraAnchor.FollowPlayer = false;

			_cameraMovementTween = GetTree().CreateTween();
			_cameraMovementTween.TweenProperty(_cameraAnchor, "global_position", new Vector2(_cameraAnchor.GlobalPosition.X, _cameraAnchor.GlobalPosition.Y + _cameraMovementDistance), 0.5f);
		}
	}

	internal override async Task OnLeaveAsync()
	{
		AnimPlayer.PlayBackwards("crouch");

		_cameraAnchor.FollowPlayer = true;

		await ToSignal(AnimPlayer, "animation_finished");
	}
}
