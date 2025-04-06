using Epilogue.Actors.Hestmor.Enums;
using Epilogue.Nodes;
using Godot;
using System.Threading.Tasks;

namespace Epilogue.Actors.Hestmor.States;
/// <inheritdoc/>
public partial class Crawl : State
{
	private readonly float _crawlSpeed;
	private readonly Player _player;

	private ShapeCast2D _slideShapeCast2D;

	/// <summary>
	/// 	State that allows Hestmor to crawl on all four
	/// </summary>
	/// <param name="stateMachine">The State Machine who owns this State</param>
	/// <param name="crawlSpeed">Speed in which Hestmor will crawl while in this State</param>
	public Crawl(StateMachine stateMachine, float crawlSpeed) : base(stateMachine)
	{
		_player = (Player)stateMachine.Owner;
		_crawlSpeed = crawlSpeed;

		SpriteSheetId = (int)Enums.SpriteSheetId.Bob;
	}

	internal override void OnStateMachineActivation()
	{
		_slideShapeCast2D = _player.ShapeCasts["Slide"];
	}

	internal override void OnInput(InputEvent @event)
	{
		if (@event.IsActionReleased("crouch_squat") && !_slideShapeCast2D.IsColliding())
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
		AnimPlayer.Play("crawl");
		AnimPlayer.Advance(0);

		_slideShapeCast2D.Enabled = true;
		_player.CanInteract = false;

		_player.TryDropGun();
	}

	internal override void PhysicsUpdate(double delta)
	{
		var movementDirection = Input.GetAxis("move_left", "move_right");

		if (movementDirection == 0f)
		{
			AnimPlayer.Pause();
		}
		else if (!AnimPlayer.IsPlaying())
		{
			AnimPlayer.Play();
		}

		var velocity = _player.Velocity;

		velocity.Y += StateMachine.Gravity * (float)delta;
		velocity.X = movementDirection * _crawlSpeed * (float)delta * 60f;

		_player.Velocity = velocity;

		_player.MoveAndSlide();

		if (!_slideShapeCast2D.IsColliding() && !Input.IsActionPressed("crouch_squat"))
		{
			StateMachine.ChangeState(typeof(Stand));
		}
	}

	internal override Task OnLeave()
	{
		_player.CanInteract = true;
		_slideShapeCast2D.Enabled = false;

		return Task.CompletedTask;
	}
}
