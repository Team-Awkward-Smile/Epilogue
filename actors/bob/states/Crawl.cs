using Epilogue.nodes;
using Godot;

namespace Epilogue.actors.hestmor.states;
/// <inheritdoc/>
public partial class Crawl : State
{
	private readonly float _crawlSpeed;
	private readonly Player _player;

	/// <summary>
	/// 	State that allows Hestmor to crawl on all four
	/// </summary>
	/// <param name="stateMachine">The State Machine who owns this State</param>
	/// <param name="crawlSpeed">Speed in which Hestmor will crawl while in this State</param>
	public Crawl(StateMachine stateMachine, float crawlSpeed) : base(stateMachine)
	{
		_player = (Player) stateMachine.Owner;
		_crawlSpeed = crawlSpeed;
	}

	internal override void OnEnter(params object[] args)
	{
		AnimPlayer.Play("crawl");
	}

	internal override void PhysicsUpdate(double delta)
	{
		var movementDirection = Input.GetAxis("move_left", "move_right");

		if(movementDirection == 0f)
		{
			AnimPlayer.Pause();
		}
		else if(!AnimPlayer.IsPlaying())
		{
			AnimPlayer.Play();
		}

		var velocity = _player.Velocity;

		velocity.Y += StateMachine.Gravity * (float) delta;
		velocity.X = movementDirection * _crawlSpeed * (float) delta * 60f;

		_player.Velocity = velocity;

		_player.MoveAndSlideWithRotation();

		var slopeNormal = _player.GetFloorNormal();
		var goingDownSlope = (movementDirection < 0 && slopeNormal.X < 0) || (movementDirection > 0 && slopeNormal.X > 0);

		if(!_player.IsOnFloor())
		{
			StateMachine.ChangeState(typeof(Fall));
		}
		else if(goingDownSlope || _player.RotationDegrees < 40f)
		{
			StateMachine.ChangeState(typeof(Idle));
		}
	}
}
