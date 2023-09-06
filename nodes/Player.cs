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

		if(_retroModeEnabled && @event.IsAction("toggle_walk_run") && @event.IsPressed())
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

	/// <inheritdoc/>
	public override void _EnterTree()
	{
		Ready += () =>
		{
			// TODO: 68 - Reset this value when the Input Mode is changed during gameplay
			_retroModeEnabled = !ProjectSettings.GetSetting("global/use_modern_controls").AsBool();
			_playerEvents = GetNode<PlayerEvents>("/root/PlayerEvents");
			_gunSystem = GetNode<GunSystem>("GunSystem");
		};
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

	/// <summary>
	///		Sweeps HeadRayCast and LedgeRayCast in search of a ledge, starting at their original positions near Hestmor's head and going down.
	/// </summary>
	/// <param name="ledgePosition">Position of the detected ledge, only valid if the method returned <c>true</c></param>
	/// <param name="sweepUntil">Y position where the sweep will stop. By default, the sweep will go down until position 0, at the pivot of Hestmor (must be a negative value)</param>
	/// <returns><c>true</c>, if a ledge is detected (in this case, <paramref name="ledgePosition"/> will contain the position of the ledge); <c>false</c>, otherwise</returns>
	public bool SweepForLedge(out Vector2 ledgePosition, int sweepUntil = 0)
	{
		var headRaycast = RayCasts["Head"];
		var ledgeRaycast = RayCasts["Ledge"];
		var originalPositions = new Vector2(headRaycast.Position.Y, ledgeRaycast.Position.Y);
		var offset = originalPositions.Y - originalPositions.X;

		ledgePosition = new Vector2(0f, 0f);

		for(var i = originalPositions.X; i <= sweepUntil; i++)
		{
			headRaycast.Position = new Vector2(0f, i);
			ledgeRaycast.Position = new Vector2(0f, headRaycast.Position.Y + offset);

			headRaycast.ForceRaycastUpdate();
			ledgeRaycast.ForceRaycastUpdate();

			if(headRaycast.IsColliding() && !ledgeRaycast.IsColliding())
			{
				var feetRaycast = RayCasts["Feet"];
				var originalTarget = feetRaycast.TargetPosition;

				feetRaycast.TargetPosition = new Vector2(0f, 30f);
				feetRaycast.ForceRaycastUpdate();

				if(feetRaycast.IsColliding())
				{
					feetRaycast.TargetPosition = originalTarget;

					continue;
				}

				ledgePosition = headRaycast.GlobalPosition;

				headRaycast.Position = new Vector2(0f, originalPositions.X);
				ledgeRaycast.Position = new Vector2(0f, originalPositions.Y);
				feetRaycast.TargetPosition = originalTarget;

				return true;
			}
		}

		headRaycast.Position = new Vector2(0f, originalPositions.X);
		ledgeRaycast.Position = new Vector2(0f, originalPositions.Y);

		return false;
	}
}
