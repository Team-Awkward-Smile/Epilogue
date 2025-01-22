using Epilogue.Actors.Hestmor.Enums;
using Epilogue.Global.Enums;
using Epilogue.Actors.Hestmor.Enums;
using Epilogue.Global.Singletons;
using Epilogue.Nodes;
using Godot;
using System.Threading.Tasks;

namespace Epilogue.Actors.Hestmor.States;
/// <inheritdoc/>
public partial class Crouch : State
{
	private readonly Player _player;
	private readonly GunEvents _gunEvents;

	private bool _crouchingAnimationFinished;

	private bool _playLeaveAnimation;

	/// <summary>
	/// 	State that allows Hestmor to crouch
	/// </summary>
	/// <param name="stateMachine">The State Machine who owns this State</param>
	/// <param name="gunEvents">Singleton responsible for emitting events related to the currently equipped Gun</param>
	public Crouch(StateMachine stateMachine, GunEvents gunEvents) : base(stateMachine)
	{
		_player = (Player)stateMachine.Owner;
		_gunEvents = gunEvents;

		SpriteSheetId = (int)Enums.SpriteSheetId.IdleWalk;
	}

	internal override void OnStateMachineActivation()
	{
		AnimPlayer.AnimationFinished += (StringName animationName) =>
		{
			if (!Active || animationName != "Crouch/crouch_begin")
			{
				return;
			}

			_crouchingAnimationFinished = true;
		};

		_gunEvents.PlayerPickedUpGun += (_, _) =>
		{
			if (!Active)
			{
				return;
			}

			StateMachine.ChangeState(typeof(Squat));
		};
	}

	internal override void OnInput(InputEvent @event)
	{
		if (@event.IsActionReleased("crouch_squat"))
		{
			StateMachine.ChangeState(typeof(Idle));
		}
		else if (!Deactivating && @event.IsActionPressed("jump"))
		{
			_player.CollisionMask &= ~(uint)CollisionLayerName.Platforms;
		}
		else if (@event.IsActionPressed("slide"))
		{
			StateMachine.ChangeState(typeof(Slide), StateType.FrontRoll);
		}
	}

	internal override void OnEnter(params object[] args)
	{
		_playLeaveAnimation = true;

		_player.CanChangeFacingDirection = false;

		AnimPlayer.Play("Crouch/crouch_begin", customSpeed: 2f);
		AudioPlayer.PlayGenericSfx("Crouch2");

		_crouchingAnimationFinished = false;
	}

	internal override void PhysicsUpdate(double delta)
	{
		_player.Velocity = new Vector2(0f, _player.Velocity.Y * StateMachine.Gravity * (float)delta);

		_player.MoveAndSlide();

		if (!_player.IsOnFloor())
		{
			_playLeaveAnimation = false;

			StateMachine.ChangeState(typeof(Fall), StateType.StandingJump, 0.5f);
		}
		
		if (_crouchingAnimationFinished && Input.GetAxis("move_left", "move_right") != 0f)
		{
			StateMachine.ChangeState(typeof(Crawl));
		}
	}

	internal override async Task OnLeave()
	{
		if (_playLeaveAnimation)
		{
			_player.CollisionMask |= (uint)CollisionLayerName.Platforms;

			AnimPlayer.PlayBackwards("crouch");

			await StateMachine.ToSignal(AnimPlayer, "animation_finished");
		}
	}
}
