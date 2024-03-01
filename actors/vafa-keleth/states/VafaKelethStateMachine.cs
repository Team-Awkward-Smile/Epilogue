using Epilogue.Extensions;
using Epilogue.Nodes;
using Godot;
using System.Linq;

namespace Epilogue.Actors.VafaKeleth.States;
public partial class VafaKelethStateMachine : NpcStateMachine
{
	[ExportGroup("Idle")]
	[Export] private float _idleMinTime = 3f;
	[Export] private float _idleMaxTime = 7f;
	[Export] private float _detectionRange = 200f;

	[ExportGroup("Combat")]
	[Export] private float _spiteFireCooldown = 5f;

	public override void _Ready()
	{
		base._Ready();

		var player = GetTree().GetLevel().Player;

		_states = new()
		{
			new Combat(this, player, _spiteFireCooldown),
			new Die(this),
			new Idle(this, _idleMinTime, _idleMaxTime, _detectionRange),
			new Jump(this),
			new Punch(this),
			new SpitFire(this),
			new TakeDamage(this),
			new Wander(this, 50f, 100f)
		};

		_currentState = _states.First(s => s.GetType() == typeof(Idle));
	}
}
