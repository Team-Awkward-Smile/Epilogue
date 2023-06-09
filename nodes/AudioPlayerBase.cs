using Godot;
using Godot.Collections;
using System.Linq;

namespace Epilogue.nodes;
[GlobalClass]
public partial class AudioPlayerBase : Node
{
	/// <summary>
	///		List of available generic SFX (grunts, hisses, screams, etc.), to be implemented by each Actor individually
	/// </summary>
	protected virtual Dictionary<string, AudioStream> GenericSfxList { get; set; }

	protected virtual Dictionary<string, AudioStream> FootstepSfxList { get; set; }

	private AudioStreamPlayer2D _genericSfxPlayer;
	private AudioStreamPlayer2D _footstepSfxPlayer;

	public override void _Ready()
	{
		// Creates an AudioStreamPlayer2D for each type of SFX available for this Actor
		if(GenericSfxList.Any())
		{
			_genericSfxPlayer = new AudioStreamPlayer2D()
			{
				Name = "GenericSFXPlayer",
				MaxPolyphony = 1,
				Bus = "SFX"
			};

			AddChild(_genericSfxPlayer);
		}

		if(FootstepSfxList.Any())
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

	public void PlaySfx(string sfxName)
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

	public void PlayFootstep(string sfxName)
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
