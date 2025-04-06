using Epilogue.Global.Enums;
using Epilogue.Global.Singletons;
using Epilogue.Guns;
using Epilogue.Guns.Flamethrower;
using Epilogue.Nodes;
using Godot;

namespace Epilogue.Actors.VafaKeleth.States;
/// <inheritdoc/>
public partial class Execution : State
{
	private readonly VafaKeleth _vafaKeleth;
	private readonly PackedScene _gunScene;
	private readonly NpcEvents _npcEvents;

	/// <summary>
	///		State that allows the Vafa'Keleth to be Executed by the player
	/// </summary>
	/// <param name="stateMachine">State Machine that owns this State</param>
	/// <param name="npcEvents">Singleton responsible for emitting events related to NPCs</param>
	public Execution(StateMachine stateMachine, NpcEvents npcEvents) : base(stateMachine)
	{
		_vafaKeleth = (VafaKeleth)stateMachine.Owner;
		_gunScene = GD.Load<PackedScene>("res://temp/flamethrower.tscn");
		_npcEvents = npcEvents;
	}

	// args[0] - ExecutionSpeed - Type of Execution that triggered this State
	internal override void OnEnter(params object[] args)
	{
		_vafaKeleth.CanAttemptSlide = false;

		var gun = _gunScene.Instantiate() as Flamethrower;

		gun.CurrentAmmoCount = (ExecutionSpeed)args[0] switch
		{
			ExecutionSpeed.Fast => gun.MaxAmmoCount / 2,
			_ => gun.MaxAmmoCount
		};

		_npcEvents.EmitSignal(NpcEvents.SignalName.GunAcquiredFromNpc, gun);
		_vafaKeleth.QueueFree();
	}
}
