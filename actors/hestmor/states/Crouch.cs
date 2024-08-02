using Epilogue.Actors.Hestmor.Enums;
using Epilogue.Global.Enums;
using Epilogue.Nodes;
using Godot;
using System.Threading.Tasks;

namespace Epilogue.Actors.Hestmor.States;
/// <inheritdoc/>
public partial class Crouch : State
{
	private readonly Player _player;

	private bool _playLeaveAnimation;

	/// <summary>
	/// 	State that allows Hestmor to crouch
	/// </summary>
	/// <param name="stateMachine">The State Machine who owns this State</param>
	public Crouch(StateMachine stateMachine) : base(stateMachine)
	{
		_player = (Player)stateMachine.Owner;

		SpriteSheetId = (int)Enums.SpriteSheetId.Bob;
	}

	internal override void OnInput(InputEvent @event)
	{
		if (@event.IsActionReleased("crouch", true))
		{
			StateMachine.ChangeState(typeof(Idle));
		}
		else if (!Deactivating && @event.IsActionPressed("jump"))
		{
			_player.CollisionMask &= ~(uint)CollisionLayerName.Platforms;
		}
	}

	internal override void OnEnter(params object[] args)
	{
		_playLeaveAnimation = true;

		_player.CanChangeFacingDirection = false;

		AnimPlayer.Play("crouch");
		AudioPlayer.PlayGenericSfx("Crouch2");
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
