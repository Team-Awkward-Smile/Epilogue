using Epilogue.nodes;
using Godot;
using Godot.Collections;

namespace Epilogue.actors.hestmor;
/// <summary>
///		Collection of sound effects used by Hestmor
/// </summary>
public partial class AudioPlayer : ActorAudioPlayer
{
	private static readonly string path = @"res://actors/bob/sfx";

	/// <inheritdoc/>
	protected override Dictionary<string, AudioStream> GenericSfxList { get; set; } = new()
	{
		{ "Slide", GD.Load<AudioStream>($"{path}//Slide_SFX.wav") },
		{ "Jump", GD.Load<AudioStream>($"{path}//JumpingStart_SFX.wav") },
		{ "Land", GD.Load<AudioStream>($"{path}//JumpingLand_SFX.wav") },
		{ "Melee", GD.Load<AudioStream>($"{path}//SwipingAttack_SFX.wav") },
		{ "GloryKill", GD.Load<AudioStream>($"{path}//glory_kill.wav") },
	};

	/// <inheritdoc/>
	protected override Dictionary<string, AudioStream> FootstepSfxList { get; set; } = new()
	{
		{ "Blood", GD.Load<AudioStream>($"{path}//blood.mp3") }
	};
}
