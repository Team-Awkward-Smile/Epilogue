using System.Linq;
using Epilogue.Global.Singletons;
using Epilogue.Nodes;
using Godot;

namespace Epilogue.Actors.Hestmor.States;
/// <summary>
/// 	The State Machine responsible for controlling Hestmor
/// </summary>
public partial class PlayerStateMachine : StateMachine
{
	[ExportGroup("Attack")]
	[Export] private float _slideAttackSpeed = 150f;

	[ExportGroup("Crawl")]
	[Export] private float _crawlSpeed = 50f;

	[ExportGroup("Growl")]
	[Export] private float _weakGrowlRadius;
	[Export] private float _mediumGrowlRadius;
	[Export] private float _strongGrowlRadius;

	[ExportGroup("Jump")]
	[ExportSubgroup("Standing Jump")]
	[Export] private float _standingJumpVerticalSpeed = -400f;
	[ExportSubgroup("Low Jump")]
	[Export] private float _lowJumpVerticalSpeed = -400f;
	[Export] private float _lowJumpHorizontalSpeed = 80f;
	[ExportSubgroup("Long Jump")]
	[Export] private float _longJumpVerticalSpeed = -400f;
	[Export] private float _longJumpHorizontalSpeed = 160f;

	[ExportGroup("Look Up")]
	[Export] private float _cameraMovementDelay = 0.5f;
	[Export] private int _cameraMovementDistance = 100;

	[ExportGroup("Run")]
	[Export] private float _runSpeed = 200f;

	[ExportGroup("Sleep")]
	[Export] private float _sleepDelay = 5f;

	[ExportGroup("Slide")]
	[ExportSubgroup("Front Roll")]
	[Export] private float _frontRollDuration = 0.5f;
	[Export] private float _frontRollSpeed = 100f;
	[ExportSubgroup("Long Slide")]
	[Export] private float _longSlideDuration = 0.5f;
	[Export] private float _longSlideSpeed = 220f;
	[ExportSubgroup("Knee Slide")]
	[Export] private float _kneeSlideDuration = 0.5f;
	[Export] private float _kneeSlideSpeed = 160f;

	[ExportGroup("Walk")]
	[Export] private float _walkSpeed = 100f;

	/// <inheritdoc/>
	public override void _Ready()
	{
		base._Ready();

		_states = new()
		{
			new Crawl(this, _crawlSpeed),
			new Crouch(this),
			new Die(this, GetNode<PlayerEvents>("/root/PlayerEvents")),
			new Fall(this),
			new GrabLedge(this),
			new Growl(this, GetNode<GrowlEffectArea>("../GrowlEffectArea"), _weakGrowlRadius, _mediumGrowlRadius, _strongGrowlRadius),
			new Idle(this, _sleepDelay),
			new Jump(this, _standingJumpVerticalSpeed, _lowJumpVerticalSpeed, _lowJumpHorizontalSpeed, _longJumpVerticalSpeed, _longJumpHorizontalSpeed),
			new LookUp(this, _cameraMovementDelay, _cameraMovementDistance),
			new MeleeAttack(this, _slideAttackSpeed),
			new Run(this, _runSpeed),
			new Sleep(this),
			new Slide(this, _frontRollDuration, _longSlideDuration, _kneeSlideDuration, _frontRollSpeed, _longSlideSpeed, _kneeSlideSpeed),
			new TakeDamage(this),
			new Vault(this),
			new Walk(this, _walkSpeed)
		};

		_currentState = _states.First(s => s.GetType() == typeof(Idle));
	}

	/// <summary>
	///		Sends the input event to the currently active State
	/// </summary>
	/// <param name="event">The input event to send to the active State</param>
	public void PropagateInputToState(InputEvent @event)
	{
		_currentState?.OnInput(@event);
	}
}
