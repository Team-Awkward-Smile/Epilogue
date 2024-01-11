using Epilogue.extensions;
using Epilogue.Global.Enums;
using Epilogue.Global.Singletons;
using Godot;
using System.Linq;

namespace Epilogue.Nodes;
/// <summary>
///		Base Node used by every gun in the game. This class can be inherited to define a more detailed execution for each gun
/// </summary>
[GlobalClass, Icon("res://nodes/icons/gun.png")]
public partial class Gun : RigidBody2D
{
	/// <summary>
	///		Maximum ammo that a gun can hold
	/// </summary>
    [Export] public int MaxAmmoCount { get; protected set; }

	/// <summary>
	///		Firing speed of the gun, in shots/minute
	/// </summary>
    [Export] public float ShotsPerMinute { get; protected set; }

	/// <summary>
	///		Time, in seconds, since this gun was fired for the last time
	/// </summary>
    public float TimeSinceLastShot { get; protected set; } = 0f;

	/// <summary>
	///		Current ammo present in the gun
	/// </summary>
    public int CurrentAmmoCount { get; protected set; }

	/// <summary>
	///		Defines if the trigger of this gun is pressed or not. This is an abstraction, the "trigger" is whatever makes the gun shoot
	/// </summary>
    public bool TriggerIsPressed 
	{
		get => _triggerIsPressed;
		set
		{
			_triggerIsPressed = value;

			if(_triggerIsPressed)
			{
				OnTriggerPress();
			}
			else
			{
				OnTriggerRelease();
			}
		}
	}

	/// <summary>
	///		The time it takes to fire a single projectile. Is equal to 1 / (<see cref="ShotsPerMinute"/> / 60)
	/// </summary>
	protected float ShotDelayPerSecond { get; private set; }

	/// <summary>
	///		Scene of the projectile used by the gun
	/// </summary>
	private protected PackedScene ProjectileScene { get; set; }

	/// <summary>
	///		Node2D representing the muzzle of the gun (i.e. where shots are spawned from)
	/// </summary>
	protected Node2D Muzzle { get; set; }

	/// <summary>
	///		Audio Player belonging to this gun. Used to play any SFX needed by the gun
	/// </summary>
	protected MultiAudioStreamPlayer AudioPlayer { get; set; }

	/// <summary>
	///		Singleton with every event triggered by the player character
	/// </summary>
	protected GunEvents GunEvents { get; set; }

	/// <summary>
	///		Main sprite used by the gun
	/// </summary>
	protected Sprite2D Sprite { get; set; }

	private bool _triggerIsPressed;

	/// <inheritdoc/>
	public override void _Ready()
	{
		Muzzle = GetNodeOrNull<Node2D>("Muzzle");

		if(Muzzle is null)
		{
			GD.PrintErr($"Muzzle not defined for Gun [{Name}]. Add a Node2D names 'Muzzle' as a child of it to fix this error");
		}

		AudioPlayer = GetChildren().OfType<MultiAudioStreamPlayer>().FirstOrDefault();

		if(AudioPlayer is null)
		{
			GD.Print($"Audio Player not defined for Gun [{Name}]");
		}

		CurrentAmmoCount = MaxAmmoCount;
		ShotDelayPerSecond = 1 / (ShotsPerMinute / 60);

		GunEvents = GetNode<GunEvents>("/root/GunEvents");
		Sprite = GetChildren().OfType<Sprite2D>().FirstOrDefault();

		AfterReady();
	}

	/// <inheritdoc/>
	public override void _Process(double delta)
	{
		if(TriggerIsPressed)
		{
			OnTriggerHeld(delta);
		}

		TimeSinceLastShot += (float) delta;
	}

	/// <summary>
	///		Method called during the frame the trigger is pressed
	/// </summary>
	private protected virtual void OnTriggerPress() { }

	/// <summary>
	///		Method called during the frame the trigger is released
	/// </summary>
	private protected virtual void OnTriggerRelease() { }

	/// <summary>
	///		Method called every frame while the trigger is held down
	/// </summary>
	private protected virtual void OnTriggerHeld(double delta) { }

	/// <summary>
	///		Method called after <see cref="_Ready"/> finishes. Used for initializing aspects unique to a specific gun
	/// </summary>
	private protected virtual void AfterReady() { }

	/// <summary>
	///		Method called whenever an empty gun is dropped. Can be overriden to implement custom logic with custom parameters
	/// </summary>
	/// <param name="args">Any parameters needed by the method</param>
	public virtual void SelfDestruct(params Variant[] args)
	{
		// Disables the collision against the player so this gun cannot be picked up while the animation plays
		GetChildren().OfType<Area2D>().First().QueueFree();

		Sprite.Material = Sprite.Material.Duplicate() as Material;

		GetTree().CreateTween().TweenMethod(Callable.From(
			(float value) => 
			{
				Sprite.SetShaderMaterialParameter("dissolveState", value);
			}), 0f, 1f, 1f)
			.Finished += QueueFree;
	}

	/// <summary>
	///		Transforms this gun into a projectile, allowing it to collide against HurtBoxes and deal damage to them
	/// </summary>
	public virtual void TransformIntoProjectile(params Variant[] args)
	{
		var pickupArea = GetChildren().OfType<Area2D>().First();

		pickupArea.CollisionLayer = (int) CollisionLayerName.PlayerHitBox;
		pickupArea.CollisionMask = (int) (CollisionLayerName.PlayerHurtBox | CollisionLayerName.World);
		pickupArea.Priority = 5;

		var impulseDirection = new Vector2(Mathf.RadToDeg(Transform[0].X), Mathf.RadToDeg(Transform[0].Y));

		ApplyImpulse(impulseDirection.Normalized() * 1000f);

		pickupArea.AreaEntered += (Area2D area) =>
		{
			if(area.Owner is Npc enemy)
			{
				enemy.DealDamage(1f);
			}

			QueueFree();
		};

		pickupArea.BodyEntered += (Node2D body) =>
		{
			SelfDestruct();
		};
	}
}
