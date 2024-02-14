using Epilogue.Global.Enums;
using Epilogue.Global.Singletons;
using Epilogue.Guns;
using Epilogue.Nodes;
using Godot;

namespace Epilogue.Actors.Icarasia.States;
/// <inheritdoc/>
public partial class Die : State
{
	private readonly Icarasia _icarasia;

	private float _timer;

	/// <summary>
	///		State used by the Icarasia when it dies
	/// </summary>
	/// <param name="stateMachine">The State Machine who owns this State</param>
	public Die(StateMachine stateMachine) : base(stateMachine)
	{
		_icarasia = (Icarasia)stateMachine.Owner;
	}

	internal override void OnEnter(params object[] args)
	{
		var executionSpeed = (args.Length > 0) ? (ExecutionSpeed?)args[0] : null;

		if (executionSpeed is not null)
		{
			var gun = GD.Load<PackedScene>("res://temp/handgun.tscn").Instantiate() as Handgun;

			gun.GlobalPosition = _icarasia.GlobalPosition;
			gun.CurrentAmmoCount = gun.MaxAmmoCount / (executionSpeed == ExecutionSpeed.Fast ? 2 : 1);

			_ = _icarasia.GetNode<NpcEvents>("/root/NpcEvents").EmitSignal(NpcEvents.SignalName.GunAcquiredFromNpc, gun);
		}
	}

	internal override void PhysicsUpdate(double delta)
	{
		_timer += (float)delta;

		if (_timer >= 1f)
		{
			_icarasia.QueueFree();
		}
	}
}
