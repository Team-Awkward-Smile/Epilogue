using Epilogue.Nodes;
using Godot;
using System.Collections.Generic;

namespace Epilogue.Actors.TadakiEiTsko.States;

/// <summary>
/// Handles the idle behavior of Tadaki Ei' Tsko (TET) based on its environment.
/// </summary>
public partial class Idle : State
{
	private readonly TadakiEiTsko _tet;
	private readonly RandomNumberGenerator _rng;
	private readonly float _detectionRange;
	private float _timer;
	private float _idleTime;
	private bool _isStruggling;
	
	public Idle(StateMachine stateMachine, float detectionRange) : base(stateMachine)
	{
		_tet = (TadakiEiTsko)stateMachine.Owner;
		_rng = new RandomNumberGenerator();
		_detectionRange = detectionRange;
	}

	internal override void OnEnter(params object[] args)
	{
		_tet.Velocity = Vector2.Zero;
		_timer = 0f;
		_idleTime = _rng.RandfRange(2f, 5f); // Random idle time before re-evaluating
		_isStruggling = false;
		
		if (_tet.IsInWater)
		{
			StartWaterIdle();
		}
		else if (_tet.IsOnLand)
		{
			StartLandIdle();
		}
		else if (_tet.IsOnString)
		{
			StartBirthingStringIdle();
		}
	}

	internal override void PhysicsUpdate(double delta)
	{
		_timer += (float)delta;
		
		if (_tet.DistanceFromPlayer <= _detectionRange)
		{
			HandleDetection();
			return;
		}

		if (_tet.IsInWater)
		{
			HandleWaterMovement(delta);
		}
	}

	private void StartWaterIdle()
	{
		// TET moves left and right under the water, leaving bubbles
		_tet.AnimPlayer.Play("underwater_idle");
		_tet.ShowBubbles();
	}

	private void HandleWaterMovement(double delta)
	{
		// Move back and forth under water
		_tet.Velocity = new Vector2(Mathf.Sin(_timer) * 50f, 0);
		_tet.MoveAndSlide();
	}

	private void StartLandIdle()
	{
		// TET sleeps on land
		_tet.AnimPlayer.Play("sleep");
	}

	private void StartBirthingStringIdle()
	{
		// TET hangs lifelessly
		_tet.AnimPlayer.Play("hanging_idle");
	}

	private void HandleDetection()
	{
		if (_tet.IsOnString)
		{
			TryDropFromString();
		}
		else if (_tet.IsInWater)
		{
			_tet.StateMachine.ChangeState(typeof(JumpAttack));
		}
		else if (_tet.IsOnLand)
		{
			_tet.StateMachine.ChangeState(typeof(MeleeAttack));
		}
	}

	private void TryDropFromString()
	{
		if (_isStruggling) return;
		
		_isStruggling = true;
		_tet.AnimPlayer.Play("struggle");
		
		_tet.GetTree().CreateTimer(1.5f).Timeout += () =>
		{
			if (_rng.RandiRange(0, 1) == 1) // 50% chance to fall
			{
				_tet.DropFromString();
				_tet.StateMachine.ChangeState(typeof(Idle)); // Becomes a land TET
			}
		};
	}
}

