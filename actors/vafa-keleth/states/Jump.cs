using Epilogue.Extensions;
using Epilogue.Global.Enums;
using Epilogue.Nodes;
using Godot;
using System.Threading.Tasks;
using static Godot.GodotObject;

namespace Epilogue.Actors.VafaKeleth.States;
public partial class Jump : State
{
	private readonly VafaKeleth _vafaKeleth;

	private bool _isJumping;
	private Vector2 _jumpDestination;
	private double _timer;
	private float _verticalDistance;

	public Jump(StateMachine stateMachine) : base(stateMachine)
	{
		_vafaKeleth = (VafaKeleth)stateMachine.Owner;
	}

	// args[0] - Vector2 - Destination of the jump
	internal override void OnEnter(params object[] args)
	{
		_jumpDestination = (Vector2)args[0];
		_verticalDistance = _jumpDestination.X - _vafaKeleth.GlobalPosition.X;
		_timer = 0f;

		_vafaKeleth.LinkNavigationAgent2D.TargetPosition = _jumpDestination;

		_vafaKeleth.TurnTowards(_jumpDestination);

		AnimPlayer.Play("jump/jump_start");

		AnimPlayer.Connect(AnimationMixer.SignalName.AnimationFinished, Callable.From((StringName anim) =>
		{
			var height = _vafaKeleth.GlobalPosition.Y - _jumpDestination.Y;
			var initialVelocity = Mathf.Sqrt(2 * StateMachine.Gravity * Mathf.Max(5f, height)) + 70f;

			if (height < 0)
			{
				_verticalDistance *= 1.2f;
				_vafaKeleth.CollisionMask &= ~(uint)CollisionLayerName.Platforms;
			}

			_vafaKeleth.Velocity = new Vector2(0f, -initialVelocity);

			_isJumping = true;

		}), (uint)ConnectFlags.OneShot);
	}

	internal override void PhysicsUpdate(double delta)
	{
		if (!_isJumping)
		{
			return;
		}

		if ((_timer += delta) >= 0.5f)
		{
			_vafaKeleth.CollisionMask |= (uint)CollisionLayerName.Platforms;
		}

		_vafaKeleth.Velocity = new Vector2(
			_verticalDistance,
			_vafaKeleth.Velocity.Y + (StateMachine.Gravity * (float)delta)
		);

		_vafaKeleth.MoveAndSlide();

		if (_vafaKeleth.Velocity.Y >= 0f)
		{
			AnimPlayer.Play("jump/fall");
		}

		if (_vafaKeleth.IsOnFloor() && _vafaKeleth.Velocity.Y >= 0)
		{
			StateMachine.ChangeState(typeof(Combat));
		}
	}

	internal override async Task OnLeave()
	{
		_isJumping = false;
		_vafaKeleth.InteractingWithNavigationLink = false;

		AnimPlayer.Play("jump/land");

		await AnimPlayer.ToSignal(AnimPlayer, AnimationMixer.SignalName.AnimationFinished);
	}
}
