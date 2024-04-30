using Epilogue.Actors.Hestmor;
using Epilogue.Actors.Hestmor.Enums;
using Epilogue.Actors.Hestmor.States;
using Epilogue.Global.Enums;
using Epilogue.Global.Singletons;
using Godot;
using System.Linq;

namespace Epilogue.Nodes;
/// <summary>
///		Node used exclusively by the Player Character
/// </summary>
[GlobalClass, Icon("res://nodes/icons/player.png")]
public partial class Player : Actor
{
	private bool _retroModeEnabled;
	private PlayerEvents _playerEvents;
	private GunSystem _gunSystem;
	private PlayerStateMachine _playerStateMachine;
	private double _quickSlideTimer;
	private bool _retroInteractHalf1 = false;
	private bool _retroInteractHalf2 = false;
	private bool _retroInteract = false;

	[Export] private bool _allowQuickSlide;

	/// <summary>
	///		Defines if the player toggled the Run mode while playing in Retro Mode
	/// </summary>
	public bool RunEnabled { get; set; } = false;

	/// <summary>
	///		Defines if Hestmor is currently holding the secret sword found in NG+
	/// </summary>
	public bool HoldingSword { get; set; } = false;

	/// <summary>
	///		Handles every input related to the player and directs it to the correct place. If the input matches nothing, it is send to the currently active State for further handling
	/// </summary>
	public override void _UnhandledInput(InputEvent @event)
	{
		if (@event.IsEcho())
		{
			return;
		}

		if (@event.IsAction("retro_interact_1"))
		{
			_retroInteractHalf1 = @event.IsPressed();
		}
		else if (@event.IsAction("retro_interact_2"))
		{
			_retroInteractHalf2 = @event.IsPressed();
		}

		_retroInteract = _retroInteractHalf1 && _retroInteractHalf2;

		if (@event.IsAction("toggle_run"))
		{
			RunEnabled = @event.IsPressed();
		}
		else if (_allowQuickSlide && @event.IsActionPressed("slide"))
		{
			_allowQuickSlide = false;

			_playerStateMachine.ChangeState(typeof(Slide), StateType.KneeSlide);
		}
		else if (HoldingSword && (@event.IsActionPressed("interact") || _retroInteract))
		{
			_playerStateMachine.ChangeState(typeof(MeleeAttack));
		}
		else if (CanInteract && _gunSystem.HasGunEquipped && @event.IsAction("shoot"))
		{
			// Tries to press/release the trigger of the equipped gun. If the gun is empty when the trigger is pressed, throw it instead
			if (!_gunSystem.InteractWithTrigger(@event.IsPressed()))
			{
				_gunSystem.ThrowGun();
			}
		}
		else if (CanInteract && (_gunSystem.IsAnyGunNearby || _gunSystem.HasGunEquipped) && (@event.IsActionPressed("interact") || _retroInteract))
		{
			_gunSystem.InteractWithGun();
		}
		else if (_retroInteract)
		{
			_retroInteract = _retroInteractHalf1 = _retroInteractHalf2 = false;

			var input = new InputEventAction()
			{
				Action = "growl",
				Pressed = true
			};

			Input.ParseInputEvent(input);
		}
		else
		{
			_playerStateMachine.PropagateInputToState(@event);
		}
	}

	/// <inheritdoc/>
	public override void _Ready()
	{
		Sprite = GetNode<Sprite2D>("%MainSprite");

		base._Ready();

		// TODO: 68 - Reset this value when the Input Mode is changed during gameplay
		_retroModeEnabled = Settings.ControlScheme == ControlScheme.Retro;
		_playerEvents = GetNode<PlayerEvents>("/root/PlayerEvents");
		_gunSystem = GetNode<GunSystem>("GunSystem");
		_playerStateMachine = GetChildren().OfType<PlayerStateMachine>().First();

		if (_playerStateMachine.ActivateOnLoad)
		{
			_playerStateMachine.Activate();
		}
	}

	/// <inheritdoc/>
	public override void _Process(double delta)
	{
		if (_allowQuickSlide)
		{
			_quickSlideTimer += delta;

			if (_quickSlideTimer >= 0.15)
			{
				_allowQuickSlide = false;
				_quickSlideTimer = 0;
			}
		}
	}

	/// <inheritdoc/>
	public override void ReduceHealth(float damage, DamageType damageType)
	{
		CurrentHealth -= damage;

		_playerEvents.EmitSignal(PlayerEvents.SignalName.PlayerWasDamaged, damage, CurrentHealth);

		if (CurrentHealth <= 0)
		{
			_playerStateMachine.ChangeState(typeof(Die));
			return;
		}
		else
		{
			_playerStateMachine.ChangeState(typeof(TakeDamage));
		}
	}

	/// <inheritdoc/>
	public override void RecoverHealth(float health)
	{
		CurrentHealth += health;

		_playerEvents.EmitSignal(PlayerEvents.SignalName.PlayerWasHealed, health, CurrentHealth);
	}

	/// <summary>
	///		Sweeps HeadRayCast and LedgeRayCast in search of a ledge, starting at their original positions near Hestmor's head and going down.
	/// </summary>
	/// <param name="ledgePosition">Position of the detected ledge, only valid if the method returned <c>true</c></param>
	/// <param name="isPlatform">Indicates if the detected ledge (if any) belongs to a platform or not</param>
	/// <param name="sweepUntil">Y position where the sweep will stop. By default, the sweep will go down until position 0, at the pivot of Hestmor (must be a negative value)</param>
	/// <returns><c>true</c>, if a ledge is detected (in this case, <paramref name="ledgePosition"/> will contain the position of the ledge, and <paramref name="isPlatform"/> will indicate if the ledge belongs to a platform); <c>false</c>, otherwise</returns>
	public bool SweepRayCastsForLedge(out Vector2 ledgePosition, out bool isPlatform, int sweepUntil = 0)
	{
		var headRaycast = RayCasts["Head"];
		var ledgeRaycast = RayCasts["Ledge"];
		var platformRaycast = RayCasts["Platform"];
		var originalPositions = new Vector2(headRaycast.Position.Y, ledgeRaycast.Position.Y);
		var offset = originalPositions.Y - originalPositions.X;

		ledgePosition = new Vector2(0f, 0f);
		isPlatform = false;

		for (var i = originalPositions.X; i <= sweepUntil; i++)
		{
			headRaycast.Position = new Vector2(0f, i);
			platformRaycast.Position = new Vector2(0f, i);
			ledgeRaycast.Position = new Vector2(0f, headRaycast.Position.Y + offset);

			headRaycast.ForceRaycastUpdate();
			platformRaycast.ForceRaycastUpdate();
			ledgeRaycast.ForceRaycastUpdate();

			if (platformRaycast.IsColliding())
			{
				do
				{
					platformRaycast.Position -= new Vector2(0f, 1f);
					platformRaycast.ForceRaycastUpdate();
				} 
				while (platformRaycast.IsColliding());

				ledgePosition = platformRaycast.GlobalPosition;
				isPlatform = true;

				headRaycast.Position = new Vector2(0f, originalPositions.X);
				platformRaycast.Position = new Vector2(0f, originalPositions.X);
				ledgeRaycast.Position = new Vector2(0f, originalPositions.Y);

				return true;

			}
			else if (headRaycast.IsColliding() && !ledgeRaycast.IsColliding())
			{
				var feetRaycast = RayCasts["Feet"];
				var originalTarget = feetRaycast.TargetPosition;

				feetRaycast.TargetPosition = new Vector2(0f, 30f);
				feetRaycast.ForceRaycastUpdate();

				if (feetRaycast.IsColliding())
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

	/// <inheritdoc/>
	public override void MoveAndSlideWithRotation()
	{
		base.MoveAndSlideWithRotation();

		_gunSystem.Rotation = -Rotation;
	}
}
