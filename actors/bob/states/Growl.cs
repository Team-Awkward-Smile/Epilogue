using Godot;
using Epilogue.Nodes;
using Epilogue.Global.Enums;
using Epilogue.Global.DTO;
using static Godot.GodotObject;

namespace Epilogue.Actors.Hestmor.States;
/// <inheritdoc/>
public partial class Growl : State
{
	private readonly float _weakGrowlRadius;
	private readonly float _mediumGrowlRadius;
	private readonly float _strongGrowlRadius;
	private readonly Player _player;
	private readonly GrowlEffectArea _growlEffectArea;

	/// <summary>
	/// 	State that allows Hestmor to growl and taunt nearby enemies
	/// </summary>
	/// <param name="stateMachine">State that allows Hestmor to grab, hang from, and climb ledges</param>
	/// <param name="growlEffectArea">Node that controls the Area2D of the Growl</param>
	/// <param name="weakGrowlRadius">The radius (in pixels) of the Weak Growl</param>
	/// <param name="mediumGrowlRadius">The radius (in pixels) of the Medium Growl</param>
	/// <param name="strongGrowlRadius">The radius (in pixels) of the Strong Growl</param>
	public Growl(
		StateMachine stateMachine,
		GrowlEffectArea growlEffectArea,
		float weakGrowlRadius,
		float mediumGrowlRadius,
		float strongGrowlRadius) : base(stateMachine)
	{
		_player = (Player)stateMachine.Owner;
		_growlEffectArea = growlEffectArea;
		_weakGrowlRadius = weakGrowlRadius;
		_mediumGrowlRadius = mediumGrowlRadius;
		_strongGrowlRadius = strongGrowlRadius;
	}

	internal override void OnEnter(params object[] args)
	{
		_player.CanChangeFacingDirection = false;

		var growlDto = _player.CurrentHealth switch
		{
			1 => new GrowlDTO()
			{
				GrowlType = GrowlType.Strong,
				Radius = _strongGrowlRadius,
				Animation = "growl_strong"
			},
			2 => new GrowlDTO()
			{
				GrowlType = GrowlType.Medium,
				Radius = _mediumGrowlRadius,
				Animation = "growl_medium"
			},
			_ => new GrowlDTO()
			{
				GrowlType = GrowlType.Weak,
				Radius = _weakGrowlRadius,
				Animation = "growl_weak"
			}
		};

		_growlEffectArea.SetUpGrowl(growlDto);

		AnimPlayer.Play($"Growl/{growlDto.Animation}");
		AnimPlayer.Connect(AnimationMixer.SignalName.AnimationFinished, Callable.From((StringName animName) => StateMachine.ChangeState(typeof(Idle))), (uint)ConnectFlags.OneShot);
	}
}
