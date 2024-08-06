using Epilogue.Actors.Hestmor.Enums;
using Epilogue.Global.Enums;
using Epilogue.Global.Singletons;
using Epilogue.Nodes;
using Godot;
using System.Threading.Tasks;

namespace Epilogue.Actors.Hestmor.States;
/// <inheritdoc/>
public partial class Squat : State
{
	private readonly Player _player;

	private CameraAnchor _cameraAnchor;
	private Tween _tween;
	private GunEvents _gunEvents;

	/// <summary>
	///		State that allows Hestmor to Squat while holding a gun, stopping her from moving but increasing the distance she can spot enemies
	/// </summary>
	/// <param name="stateMachine">The State Machine who owns this State</param>
	public Squat(StateMachine stateMachine, GunEvents gunEvents) : base(stateMachine)
	{
		_player = (Player)stateMachine.Owner;
		_gunEvents = gunEvents;

		SpriteSheetId = (int)Enums.SpriteSheetId.Squat;
	}

	internal override void OnStateMachineActivation()
	{
		_gunEvents.GunWasDropped += () =>
		{
			if (!Active)
			{
				return;
			}

			StateMachine.ChangeState(typeof(Crouch));
		};
	}

	internal override void OnInput(InputEvent @event)
	{
		if (@event.IsActionReleased("crouch_squat"))
		{
			StateMachine.ChangeState(typeof(Idle));
		}
		else if (@event.IsActionPressed("slide"))
		{
			StateMachine.ChangeState(typeof(Slide), StateType.FrontRoll);
		}
	}

	internal override void OnEnter(params object[] args)
	{
		AnimPlayer.Play("squat");

		_cameraAnchor = _player.GetNode<CameraAnchor>("CameraAnchor");

		_player.CanChangeFacingDirection = false;
		_cameraAnchor.FollowPlayer = false;

		_tween = StateMachine.GetTree().CreateTween();

		_tween.TweenProperty(_cameraAnchor, "position", new Vector2(150f * (_player.FacingDirection == ActorFacingDirection.Left ? -1 : 1), 0f), 0.3f);
	}

	internal override Task OnLeave()
	{
		if (_tween.IsRunning())
		{
			_tween.Kill();
		}

		_player.CanChangeFacingDirection = true;
		_cameraAnchor.FollowPlayer = true;

		return Task.CompletedTask;
	}
}
