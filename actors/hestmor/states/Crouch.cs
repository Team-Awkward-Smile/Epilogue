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

	/// <summary>
	/// 	State that allows Hestmor to crouch
	/// </summary>
	/// <param name="stateMachine">The State Machine who owns this State</param>
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
			StateMachine.ChangeState(typeof(Stand));
		}
		else if (@event.IsActionPressed("slide"))
		{
			StateMachine.ChangeState(typeof(Slide), StateType.FrontRoll);
		}
	}

	internal override void OnEnter(params object[] args)
	{
		_player.CanChangeFacingDirection = false;

		AnimPlayer.Play("Crouch/crouch_begin", customSpeed: 2f);
		AudioPlayer.PlayGenericSfx("Crouch2");

		_crouchingAnimationFinished = false;
	}

	internal override void PhysicsUpdate(double delta)
	{
		if (_crouchingAnimationFinished && Input.GetAxis("move_left", "move_right") != 0f)
		{
			StateMachine.ChangeState(typeof(Crawl));
		}
	}
}
