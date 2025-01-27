using Epilogue.Extensions;
using Epilogue.Global.Singletons;
using Epilogue.Nodes;
using Godot;
using System.Linq;

namespace Epilogue.Actors.VafaKeleth.States;
/// <summary>
///		StateMachine Node used by Vafa'Keleth
/// </summary>
public partial class VafaKelethStateMachine : NpcStateMachine
{
	[ExportGroup("Idle")]
	[Export] private float _idleMinTime = 3f;
	[Export] private float _idleMaxTime = 7f;
	[Export] private float _detectionRange = 200f;

	[ExportGroup("Combat")]
	[Export] private float _spiteFireCooldown = 5f;

	[ExportGroup("Slide")]
	[Export] private float _slideSpeed = 80f;

	/// <inheritdoc/>
	public override void _Ready()
	{
		base._Ready();

		var player = GetTree().GetLevel().Player;

		_states = new()
		{
			new Combat(this, _spiteFireCooldown),
			new Die(this),
			new Execution(this, GetNode<NpcEvents>("/root/NpcEvents")),
			new Idle(this, _idleMinTime, _idleMaxTime, _detectionRange),
			new Jump(this),
			new Punch(this),
			new Slide(this, _slideSpeed),
			new SpitFire(this),
			new Stun(this),
			new TakeDamage(this),
			new Vulnerable(this),
			new WalkBack(this, player, 50f),
			new Wander(this)
		};

		_currentState = _states.First(s => s.GetType() == typeof(Idle));

		_currentState.Active = true;
	}
}
