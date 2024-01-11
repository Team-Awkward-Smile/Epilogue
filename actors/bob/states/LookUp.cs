using Godot;
using Epilogue.Nodes;
using System.Threading.Tasks;

namespace Epilogue.actors.hestmor.states;
/// <inheritdoc/>
public partial class LookUp : State
{
	private readonly float _cameraMovementDelay;
	private readonly int _cameraMovementDistance;
	private readonly Player _player;

	private CameraAnchor _cameraAnchor;
	private Tween _raiseCameraTween;
	private bool _isCameraMoving = false;
	private float _timer = 0f;

	/// <summary>
	/// 	State that allows Hestmor to look up
	/// </summary>
	/// <param name="stateMachine">The State Machine who owns this State</param>
	/// <param name="cameraMovementDelay">The time it takes (in seconds) for the Camera to start moving</param>
	/// <param name="cameraMovementDistance">The distance (in pixels) the Camera will travel vertically up</param>
	public LookUp(StateMachine stateMachine, float cameraMovementDelay, int cameraMovementDistance) : base(stateMachine)
	{
		_player = (Player) StateMachine.Owner;
		_cameraMovementDelay = cameraMovementDelay;
		_cameraMovementDistance = cameraMovementDistance;
	}

	internal override void OnInput(InputEvent @event)
	{
		if(Input.IsActionJustReleased("look_up"))
		{
			_raiseCameraTween?.Stop();

			StateMachine.ChangeState(typeof(Idle));
		}
	}

	internal override void OnEnter(params object[] args)
	{
		_isCameraMoving = false;
		_timer = 0f;
		_cameraAnchor = _player.GetNode<CameraAnchor>("CameraAnchor");
	}

	internal override void Update(double delta)
	{
		_timer += (float) delta;

		if(_timer > _cameraMovementDelay && !_isCameraMoving)
		{
			_cameraAnchor.FollowPlayer = false;
			_isCameraMoving = true;
			_raiseCameraTween = StateMachine.GetTree().CreateTween();
			_raiseCameraTween.TweenProperty(_cameraAnchor, "position", new Vector2(_cameraAnchor.Position.X, _cameraAnchor.Position.Y - _cameraMovementDistance), 0.5f);
		}
	}

	internal override Task OnLeave()
	{
		_cameraAnchor.FollowPlayer = true;

		return Task.CompletedTask;
	}
}

