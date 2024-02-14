using Epilogue.Extensions;
using Epilogue.Nodes;
using Godot;
using System.Linq;

namespace Epilogue.Actors.Icarasia.States;
/// <summary>
///		State Machine used by the Icarasia
/// </summary>
public partial class IcarasiaStateMachine : NpcStateMachine
{
	[ExportGroup("General")]
	[Export] private float _fleeRadius = 50f;

	// ----------------------------------------------

	[ExportGroup("Move")]
	[Export] private float _moveSpeed = 80f;

	// ----------------------------------------------

	[ExportGroup("Shoot")]
	[Export] private float _shotCooldown = 5f;
	[Export] private float _shotDesiredDistance = 100f;

	// ----------------------------------------------

	[ExportGroup("Stinger")]
	[Export] private float _stingerCooldown = 3f;

	// ----------------------------------------------

	[ExportGroup("Wander")]
	[Export] private float _wanderSpeed = 50f;
	[Export] private float _wanderFlipTime = 3f;

	// ----------------------------------------------

	[ExportGroup("Stun")]
	[Export] private float _stunDuration = 5f;

	// ----------------------------------------------

	[ExportGroup("Charge")]
	[Export] private float _chargeSpeed = 250f;
	[Export] private float _chargeDuration = 2f;
	[Export] private float _recoveryTime = 3f;

	/// <inheritdoc/>
	public override void _Ready()
	{
		base._Ready();

		var player = GetTree().GetLevel().Player;
		var icarasia = (Icarasia)Owner;

		_states = new()
		{
			new Charge(this, player, _chargeSpeed, _chargeDuration),
			new Die(this),
			new Flee(this, player),
			new Idle(this),
			new Move(this, player, _moveSpeed, _shotDesiredDistance),
			new Push(this),
			new Recover(this, _recoveryTime),
			new Shoot(this),
			new Sting(this),
			new Stun(this, _stunDuration),
			new Vulnerable(this),
			new Wander(this, _wanderSpeed)
		};

		_currentState = _states.First(s => icarasia.NearTerraBischem ? s.GetType() == typeof(Idle) : s.GetType() == typeof(Wander));

		icarasia.ShotCooldown = _shotCooldown;
		icarasia.StingerCooldown = _stingerCooldown;
	}
}
