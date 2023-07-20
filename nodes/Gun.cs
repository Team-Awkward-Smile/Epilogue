using Epilogue.global.singletons;
using Godot;
using System.Linq;

namespace Epilogue.nodes;
public abstract partial class Gun : RigidBody2D
{
    [Export] public int MaxAmmoCount { get; set; }
    [Export] public float ShotsPerMinute { get; set; }

    public float TimeSinceLastShot { get; protected set; } = 0f;
    public int CurrentAmmoCount { get; protected set; }

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

    protected Node2D Muzzle { get; set; }
	protected AudioStreamPlayer AudioPlayer { get; set; }
    protected Events Events { get; set; }

	private bool _triggerIsPressed;

	public override void _Ready()
	{
		Muzzle = GetNodeOrNull<Node2D>("Muzzle");

		if(Muzzle is null)
		{
			GD.PrintErr($"Muzzle not defined for Gun [{Name}]. Add a Node2D names 'Muzzle' as a child of it to fix this error");
		}

		AudioPlayer = GetChildren().OfType<AudioStreamPlayer>().FirstOrDefault();

		if(AudioPlayer is null)
		{
			GD.Print($"Audio Player not defined for Gun [{Name}]");
		}

		CurrentAmmoCount = MaxAmmoCount;

		Events = GetNode<Events>("/root/Events");
	}

	public override void _Process(double delta)
	{
		if(TriggerIsPressed)
		{
			OnTriggerHeld(delta);
		}

		TimeSinceLastShot += (float) delta;
	}

	/// <summary>
	///		Code that runs during the frame the trigger is pressed
	/// </summary>
	public virtual void OnTriggerPress() { }

	/// <summary>
	///		Code that runs during the frame the trigger is released
	/// </summary>
	public virtual void OnTriggerRelease() { }

	/// <summary>
	///		Code that runs every frame while the trigger is held
	/// </summary>
	public virtual void OnTriggerHeld(double delta) { }
}
