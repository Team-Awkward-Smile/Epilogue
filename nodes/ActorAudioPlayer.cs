using Godot;
using Godot.Collections;
using System.Linq;

namespace Epilogue.Nodes;
/// <summary>
///		Base class for AudioPlayers used by Actors
/// </summary>
[GlobalClass, Tool]
public partial class ActorAudioPlayer : Node
{
	/// <summary>
	///		List of available generic SFX (grunts, hisses, screams, etc.), to be implemented by each Actor individually
	/// </summary>
	protected virtual Dictionary<string, AudioStream> GenericSfxList { get; set; }

	/// <summary>
	///		List of available footsteps SFX to be implemented by each Actor individually
	/// </summary>
	protected virtual Dictionary<string, AudioStream> FootstepSfxList { get; set; }

	private AudioStreamPlayer2D _genericSfxPlayer;
	private AudioStreamPlayer2D _footstepSfxPlayer;

	/// <inheritdoc/>
	public override void _Ready()
	{
		if(Engine.IsEditorHint())
		{
			return;
		}

		// Creates an AudioStreamPlayer2D for each type of SFX available for this Actor
		if(GenericSfxList?.Any() == true)
		{
			_genericSfxPlayer = new AudioStreamPlayer2D()
			{
				Name = "GenericSFXPlayer",
				MaxPolyphony = 1,
				Bus = "SFX"
			};

			AddChild(_genericSfxPlayer);
		}

		if(FootstepSfxList?.Any() == true)
		{
			_footstepSfxPlayer = new AudioStreamPlayer2D()
			{
				Name = "FootstepSFXPlayer",
				MaxPolyphony = 3,
				Bus = "SFX"
			};

			AddChild(_footstepSfxPlayer);
		}
	}

	/// <summary>
	///		Plays a predefined generic SFX from the <see cref="GenericSfxList"/> list belonging to the Actor who owns this Node
	/// </summary>
	/// <param name="sfxName">Name of the Generic SFX</param>
	public void PlayGenericSfx(string sfxName)
	{
		if(GenericSfxList.TryGetValue(sfxName, out var sfx))
		{
			_genericSfxPlayer.Stream = sfx;
			_genericSfxPlayer.Play();
		}
		else
		{
			GD.PushWarning($"SFX [{sfxName}] not found for Actor [{Owner.Name}]");
		}
	}

	/// <summary>
	///		Plays a predefined footstep SFX from the <see cref="FootstepSfxList"/> list belonging to the Actor who owns this Node
	/// </summary>
	/// <param name="sfxName">Name of the Footstep SFX</param>
	public void PlayFootstepSfx(string sfxName)
	{
		if(FootstepSfxList.TryGetValue(sfxName, out var sfx))
		{
			_footstepSfxPlayer.Stream = sfx;
			_footstepSfxPlayer.Play();
		}
		else
		{
			GD.PushWarning($"Footstep [{sfxName}] not found for Actor [{Owner.Name}]");
		}
	}
}
