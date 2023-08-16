using Godot;
using Epilogue.nodes;
using System.Threading.Tasks;

namespace Epilogue.actors.hestmor.states;
/// <summary>
///		State that allows Hestmor to look up
/// </summary>
public partial class LookUp : PlayerState
{
	[Export] private float _cameraMovementDelay = 0.5f;
	[Export] private int _cameraMovementDistance = 100;

	private CameraAnchor _cameraAnchor;
	private Tween _raiseCameraTween;
	private bool _isCameraMoving = false;
	private float _timer = 0f;

	internal override void OnInput(InputEvent @event)
	{
		if(@event.IsActionReleased(LookUpInput))
		{
			_raiseCameraTween?.Stop();

			StateMachine.ChangeState("Idle");
		}
	}

	internal override void OnEnter()
	{
		_isCameraMoving = false;
		_timer = 0f;
		_cameraAnchor = Player.GetNode<CameraAnchor>("CameraAnchor");
	}

	internal override void Update(double delta)
	{
		_timer += (float) delta;

		if(_timer > _cameraMovementDelay && !_isCameraMoving)
		{
			_cameraAnchor.FollowPlayer = false;
			_isCameraMoving = true;
			_raiseCameraTween = GetTree().CreateTween();
			_raiseCameraTween.TweenProperty(_cameraAnchor, "position", new Vector2(_cameraAnchor.Position.X, _cameraAnchor.Position.Y - _cameraMovementDistance), 0.5f);
		}
	}

	internal override void OnLeave()
	{
		_cameraAnchor.FollowPlayer = true;
	}
}

