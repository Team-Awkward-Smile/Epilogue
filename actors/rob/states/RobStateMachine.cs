using System.Linq;
using Epilogue.extensions;
using Epilogue.nodes;
using Godot;

namespace Epilogue.actors.rob.states;
/// <summary>
/// 	State Machine exclusive to Rob
/// </summary>
public partial class RobStateMachine : NpcStateMachine
{
	[ExportSubgroup("Wander"), Export] private float _wanderSpeed = 50f;
	[ExportSubgroup("Attacks"), Export] private float _shotCooldown = 5f;
	[Export] private float _meleeCooldown = 2f;
	[ExportSubgroup("Movement"), Export] private float _moveSpeed = 80f;
	[Export] private float _fleeSpeed = 100f;

	/// <inheritdoc/>
	public override void _Ready()
	{
		base._Ready();

		var player = GetTree().GetLevel().Player;
		var rob = (Rob) Owner;

		_states = new()
		{
			new Attack(this),
			new Die(this),
			new Executed(this),
			new Flee(this, _fleeSpeed),
			new Move(this, player, _moveSpeed),
			new Shoot(this, player),
			new Stun(this),
			new Wander(this, _wanderSpeed)
		};

		_currentState = _states.First(s => s.GetType() == typeof(Move));

		rob.ShotCooldown = _shotCooldown;
		rob.MeleeCooldown = _meleeCooldown;
	}
}
