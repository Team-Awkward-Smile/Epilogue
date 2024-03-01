using Epilogue.Extensions;
using Epilogue.Nodes;
using Godot;
using Godot.Collections;

namespace Epilogue.Actors.VafaKeleth.States;
public partial class Combat : State
{
	private readonly VafaKeleth _vafaKeleth;
	private readonly Player _player;
	private readonly double _spitFireCooldown;

	private ShapeCast2D _fireStreamShapeCast;
	private double _sweepTimer;
	private bool _isMoving = false;

	public Combat(StateMachine stateMachine, Player player, double spitFireCooldown) : base(stateMachine)
	{
		_vafaKeleth = (VafaKeleth)stateMachine.Owner;
		_player = player;
		_spitFireCooldown = spitFireCooldown;
	}

	internal override void OnStateMachineActivation()
	{
		_vafaKeleth.PlayerNavigationAgent2D.LinkReached += (Dictionary details) =>
		{
			if (!_vafaKeleth.InteractingWithNavigationLink)
			{
				_vafaKeleth.InteractingWithNavigationLink = true;

				StateMachine.ChangeState(typeof(Jump), (Vector2)details["link_exit_position"]);
			}
		};
	}

	internal override void OnEnter(params object[] args)
	{
		AnimPlayer.Play("idle");

		_isMoving = false;
		_fireStreamShapeCast = _vafaKeleth.ShapeCasts["Attack"];
	}

	internal override void Update(double delta)
	{
		if ((_sweepTimer += delta) >= 0.1)
		{
			_sweepTimer = 0;

			if (SweepShapeCastForPlayer(out var hitAngle, out var hitDistance) && _vafaKeleth.TimeSinceLastAttack >= _spitFireCooldown)
			{
				_vafaKeleth.TimeSinceLastAttack = 0;

				if (hitDistance >= 50f)
				{
					StateMachine.ChangeState(typeof(SpitFire), hitAngle.Value);
				}
				else
				{
					StateMachine.ChangeState(typeof(Punch));
				}

				return;
			}
		}
	}

	internal override void PhysicsUpdate(double delta)
	{
		_vafaKeleth.Velocity = _fireStreamShapeCast.IsColliding() ?
			Vector2.Zero :
			_vafaKeleth.PlayerNavigationAgent2D.GetNextVelocity(_vafaKeleth.GlobalPosition, 100f);

		_vafaKeleth.Velocity += new Vector2(0f, _vafaKeleth.Velocity.Y + (StateMachine.Gravity * (float)delta));

		_vafaKeleth.MoveAndSlide();

		if (!_isMoving && _vafaKeleth.Velocity.X != 0f)
		{
			_isMoving = true;

			AnimPlayer.Play("walk");
		}
		else if (_isMoving && _vafaKeleth.Velocity.X == 0f)
		{
			_isMoving = false;

			AnimPlayer.Play("idle");
		}

		if (_vafaKeleth.Velocity.X != 0f)
		{
			_vafaKeleth.TurnTowards(_vafaKeleth.GlobalPosition + _vafaKeleth.Velocity);
		}
	}

	private bool SweepShapeCastForPlayer(out int? hitAngle, out float? hitDistance)
	{
		foreach (int angle in _vafaKeleth.AttackAngles)
		{
			_fireStreamShapeCast.RotationDegrees = angle;

			_fireStreamShapeCast.ForceShapecastUpdate();

			if (_fireStreamShapeCast.IsColliding())
			{
				_fireStreamShapeCast.RotationDegrees = 0;

				hitAngle = angle;
				hitDistance = _vafaKeleth.GlobalPosition.DistanceTo(_fireStreamShapeCast.GetCollisionPoint(0));

				return true;
			}
		}

		_fireStreamShapeCast.RotationDegrees = 0;

		hitAngle = null;
		hitDistance = null;

		return false;
	}
}
