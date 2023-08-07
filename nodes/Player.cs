using Epilogue.actors.hestmor;
using Epilogue.extensions;
using Epilogue.global.enums;
using Epilogue.global.singletons;
using Epilogue.util;

using Godot;

using System.Linq;

namespace Epilogue.nodes;
/// <summary>
///		Node used exclusively by the Player Character
/// </summary>
[GlobalClass, Icon("res://nodes/icons/player.png")]
public partial class Player : Actor
{
	private bool _retroModeEnabled;
	private PlayerEvents _playerEvents;
	private GunSystem _gunSystem;

	/// <summary>
	///		Defines if the player toggled the Run mode while playing in Retro Mode
	/// </summary>
	public bool RunEnabled { get; set; } = false;

	/// <summary>
	///		Handles every input related to the player and directs it to the correct place. If the input matches nothing, it is send to the currently active State for further handling
	/// </summary>
	public override void _UnhandledInput(InputEvent @event)
	{
		if(@event.IsEcho())
		{
			return;
		}

		if(_retroModeEnabled && @event.IsAction(InputUtils.GetInputActionName("toggle_walk_run")))
		{
			RunEnabled = !RunEnabled;
		}
		else if(@event.IsAction(InputUtils.GetInputActionName("run_modifier")))
		{
			RunEnabled = @event.IsPressed();
		}
		else if(_gunSystem.HasGunEquipped && @event.IsAction(InputUtils.GetInputActionName("shoot")))
		{
			// Tries to press/release the trigger of the equipped gun. If the gun is empty when the trigger is pressed, throw it instead
			if(!_gunSystem.InteractWithTrigger(@event.IsPressed()))
			{
				_gunSystem.ThrowGun();
			}
		}
		else if((_gunSystem.IsAnyGunNearby || _gunSystem.HasGunEquipped) && @event.IsActionPressed(InputUtils.GetInputActionName("pickup_or_drop_gun")))
		{
			_gunSystem.InteractWithGun();
		}
		else
		{
			StateMachine.PropagateInputToState(@event);
		}
	}

    private protected override void AfterReady()
	{
		// TODO: 68 - Reset this value when the Input Mode is changed during gameplay
		_retroModeEnabled = !ProjectSettings.GetSetting("global/use_modern_controls").AsBool();
		_playerEvents = GetNode<PlayerEvents>("/root/PlayerEvents");
		_gunSystem = GetNode<GunSystem>("GunSystem");
	}

	/// <inheritdoc/>
	public override void DealDamage(float damage)
	{
		CurrentHealth -= damage;

		if(CurrentHealth <= 0)
		{
			StateMachine.ChangeState("Die");
			return;
		}
		else
		{
			StateMachine.ChangeState("TakeDamage");
		}

		Sprite.SetShaderMaterialParameter("iframeActive", true);

		GetChildren().OfType<HurtBox>().First().CollisionMask = 0;

		GetTree().CreateTimer(1f).Timeout += () =>
		{
			Sprite.SetShaderMaterialParameter("iframeActive", false);

			GetChildren().OfType<HurtBox>().First().CollisionMask = (int) CollisionLayerName.HitBoxes;
		};
	}

	/// <inheritdoc/>
	public override void ApplyHealth(float health)
	{
		CurrentHealth += health;
	}
}
