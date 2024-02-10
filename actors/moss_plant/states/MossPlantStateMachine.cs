using Epilogue.Nodes;
using Godot;
using System.Linq;

namespace Epilogue.Actors.MossPlant.States;
/// <summary>
///		State Machine used by the Moss Plant of Guwama
/// </summary>
public partial class MossPlantStateMachine : NpcStateMachine
{
	[ExportGroup("Combat")]
	[Export] private float _projectileCooldown = 5f;

	/// <inheritdoc/>
	public override void _Ready()
	{
		_states = new()
		{
			new Combat(this, _projectileCooldown),
			new Die(this),
			new Idle(this),
			new Shoot(this)
		};

		_currentState = _states.First(s => s.GetType() == typeof(Idle));
	}
}
