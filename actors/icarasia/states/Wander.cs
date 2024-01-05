using Epilogue.Global.Enums;
using Epilogue.Nodes;
using Godot;
using System.Threading.Tasks;

namespace Epilogue.Actors.Icarasia.States;
/// <inheritdoc/>
public partial class Wander : State
{
	private readonly Icarasia _icarasia;
	private readonly float _wanderSpeed;
	private readonly RandomNumberGenerator _rng;

	private float _timer;
	private RayCast2D _wanderRaycast;

	/// <summary>
	///		State used by the Icarasia before the player is detected
	/// </summary>
	/// <param name="stateMachine">State Machine who owns this State</param>
	/// <param name="wanderSpeed">Speed (in units) the Icarasia will move while wandering</param>
	public Wander(StateMachine stateMachine, float wanderSpeed) : base(stateMachine)
	{
		_icarasia = (Icarasia)stateMachine.Owner;
		_wanderSpeed = wanderSpeed;
		_rng = new RandomNumberGenerator();
	}

	internal override void OnEnter(params object[] args)
	{
		_wanderRaycast = _icarasia.RayCasts["Wander"];

		_wanderRaycast.Enabled = true;
		_wanderRaycast.ForceRaycastUpdate();

		if (_wanderRaycast.IsColliding() || (args.Length > 0 && (bool)args[0] && _rng.Randi() % 2 == 0))
		{
			_icarasia.FlipFacingDirection();
		}

		_icarasia.Velocity = new Vector2(_wanderSpeed * (_icarasia.FacingDirection == ActorFacingDirection.Right ? 1 : -1), 0f);
		_timer = 0f;
	}

	internal override void PhysicsUpdate(double delta)
	{
		_ = _icarasia.MoveAndSlide();

		_timer += (float)delta;

		_wanderRaycast.ForceRaycastUpdate();

		if (_timer >= _rng.RandfRange(2f, 8f) || _wanderRaycast.IsColliding())
		{
			StateMachine.ChangeState(typeof(Idle), _rng.RandfRange(1f, 4f));
		}
		else if (_icarasia.IsPlayerDetected)
		{
			StateMachine.ChangeState(typeof(Move));
		}
	}

	internal override Task OnLeave()
	{
		_wanderRaycast.Enabled = false;

		return Task.CompletedTask;
	}
}
