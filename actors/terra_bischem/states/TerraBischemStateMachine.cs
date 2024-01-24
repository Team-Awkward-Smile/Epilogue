using Epilogue.Extensions;
using Epilogue.Nodes;
using Godot;
using System.Linq;

namespace Epilogue.Actors.TerraBischem.States;
public partial class TerraBischemStateMachine : NpcStateMachine
{
	[ExportCategory("General")]
	[Export] private float _detectionRange = 200f;

	[ExportCategory("Combat")]
	[Export] private float _attackRange = 100f;
	[Export] private float _attackCooldown = 5f;

	public override void _Ready()
	{
		var player = GetTree().GetLevel().Player;

		_states = new()
		{
			new Attack(this, player, _attackRange),
			new Combat(this, player, _attackCooldown, _attackRange),
			new Idle(this, _detectionRange)
		};

		_currentState = _states.First(s => s.GetType() == typeof(Idle));
	}
}
