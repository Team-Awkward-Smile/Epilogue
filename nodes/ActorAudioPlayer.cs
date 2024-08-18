using Godot;
using Godot.Collections;
using System.Linq;
using System.Text.RegularExpressions;

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
	///		List of available footstep SFX to be implemented by each Actor individually
	/// </summary>
	protected virtual Dictionary<string, AudioStream> FootstepSfxList { get; set; }

	/// <summary>
	///		List of available collision SFX to be implemented by each Actor individually
	/// </summary>
	protected virtual Dictionary<string, AudioStream> CollisionSfxList { get; set; }

	private AudioStreamPlayer2D _genericSfxPlayer;
	private AudioStreamPlayer2D _footstepSfxPlayer;
	private AudioStreamPlayer2D _collisionSfxPlayer;

	/// <summary>
	/// return a bool if the specified audioplayer is playing a sound
	/// </summary>
	/// <param name="audioPlayer"></param>
	/// <returns></returns>
	public bool HasStreamPlayback(string audioPlayer)
	{
		switch (audioPlayer)
		{
			case "generic":
				return _genericSfxPlayer.HasStreamPlayback();
			case "footsteps":
				return _footstepSfxPlayer.HasStreamPlayback();
			case "collision":
				return _collisionSfxPlayer.HasStreamPlayback();
			default:
				return false;	
		}
	}

	public void Stop(string audioPlayer)
	{	
		switch (audioPlayer)
		{
			case "generic":
				_genericSfxPlayer.Stop();
				break;
			case "footsteps":
				_footstepSfxPlayer.Stop();
				break;
			case "collision":
				_collisionSfxPlayer.Stop();
				break;
			default:
				break;
		}
	}
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
				Bus = "Footsteps"
			};

			AddChild(_footstepSfxPlayer);
		}

		if(CollisionSfxList?.Any() == true)
		{
			_collisionSfxPlayer = new AudioStreamPlayer2D()
			{
				Name = "CollisionSFXPlayer",
				MaxPolyphony = 3,
				Bus = "SFX"
			};

			AddChild(_collisionSfxPlayer);
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
	///		Plays a predefined footstep SFX from the <see cref="CollisionSfxList"/> list belonging to the Actor who owns this Node
	/// </summary>
	/// <param name="sfxName">Name of the Footstep SFX</param>
	public void PlayCollisionSfx(string sfxName)
	{
		if(CollisionSfxList.TryGetValue(sfxName, out var sfx))
		{
			_collisionSfxPlayer.Stream = sfx;
			_collisionSfxPlayer.Play();
		}
		else
		{
			GD.PushWarning($"Footstep [{sfxName}] not found for Actor [{Owner.Name}]");
		}
	}

	/// <summary>
	///		Plays a random Collision SFX whose name starts with <paramref name="prefix"/>
	/// </summary>
	/// <param name="prefix">The prefix of the category of SFX to be played</param>
	public void PlayRandomCollisionSfx(string prefix)
	{
		var rng = new RandomNumberGenerator();
		var possibleSfx = CollisionSfxList.Where(sfx => sfx.Key.StartsWith(prefix));
		var sfx = possibleSfx.ElementAt(rng.RandiRange(0, possibleSfx.Count() - 1)).Value;

		_collisionSfxPlayer.Stream = sfx;
		_collisionSfxPlayer.Play();
	}

	/// <summary>
	///		Plays a random Footstep SFX whose name starts with <paramref name="prefix"/>
	/// </summary>
	/// <param name="prefix">The prefix of the category of the SFX to be played</param>
	public void PlayRandomFootstepSfx(string prefix)
	{
		var rng = new RandomNumberGenerator();
		var possibleSfx = FootstepSfxList.Where(sfx => sfx.Key.StartsWith(prefix));
		var sfx = possibleSfx.ElementAt(rng.RandiRange(0, possibleSfx.Count() - 1)).Value;

		_footstepSfxPlayer.Stream = sfx;
		_footstepSfxPlayer.Play();
	}
}
