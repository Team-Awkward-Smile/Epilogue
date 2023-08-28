using Epilogue.global.enums;

using Godot;

namespace Epilogue.nodes;
/// <summary>
///		Node used to save point is the level where the player can respawn
/// </summary>
[GlobalClass, Icon("res://nodes/icons/checkpoint.png"), Tool]
public partial class Checkpoint : Area2D
{
	/// <summary>
	///		ID of this Checkpoint in the Level, used to find this Checkpoint when the Level reloads
	/// </summary>
    public int ID { get; set; }

    /// <summary>
    ///     Defines if this Checkpoint is the current one where the player will respawn
    /// </summary>
    public bool Current { get; set; } = false;

	/// <summary>
	///		Defines if this Checkpoint will be the default one used when a Level starts
	/// </summary>
	[Export] public bool FirstCheckpoint 
	{
		get => _firstCheckpoint;
		set
		{
			_firstCheckpoint = value;
			GetParent()?.GetParent()?.UpdateConfigurationWarnings();
		}
	}

	private bool _firstCheckpoint = false;

	/// <summary>
	///		Event triggered whenever this Checkpoint is trigger3ed by the player
	/// </summary>
	[Signal] public delegate void CheckpointTriggeredEventHandler();

	/// <inheritdoc/>
	public override void _Ready()
	{
		if(Engine.IsEditorHint())
		{
			return;
		}

		BodyEntered += (Node2D body) =>	EmitSignal(SignalName.CheckpointTriggered);

		if(ProjectSettings.GetSetting("debug/show_checkpoints").AsBool())
		{
			SetCheckpointState(CheckpointState.Inactive);
		}
		else
		{
			GetNode<Sprite2D>("Sprite2D").QueueFree();
		}
	}

	/// <summary>
	///		Sets a new state for this Checkpoint. Used only for visual debugging, has no effect on how the Checkpoint works
	/// </summary>
	/// <param name="state"></param>
	public void SetCheckpointState(CheckpointState state)
	{
		if(!ProjectSettings.GetSetting("debug/show_checkpoints").AsBool())
		{
			return;
		}

		GetNode<Sprite2D>("Sprite2D").Modulate = state switch
		{
			CheckpointState.Inactive => new Color(141f / 255, 165f / 255, 243f / 255),
			CheckpointState.Current => new Color(226f / 255, 196f / 255, 22f / 255),
			_ => new Color(209f / 255, 33f / 255, 49f / 255)
		};
	}
}
