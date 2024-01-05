using Epilogue.Nodes;
using Godot;
using Godot.Collections;

namespace Epilogue.Actors.Hestmor;
/// <summary>
///		Collection of sound effects used by Hestmor
/// </summary>
public partial class AudioPlayer : ActorAudioPlayer
{
	private static readonly string PATH = @"res://actors/bob/sfx";

	/// <inheritdoc/>
	protected override Dictionary<string, AudioStream> GenericSfxList { get; set; } = new()
	{
		{ "Slide", GD.Load<AudioStream>($"{PATH}//Slide_SFX.wav") },
		{ "Jump", GD.Load<AudioStream>($"{PATH}//JumpingStart_SFX.wav") },
		{ "Land", GD.Load<AudioStream>($"{PATH}//JumpingLand_SFX.wav") },
		{ "Melee", GD.Load<AudioStream>($"{PATH}//SwipingAttack_SFX.wav") },
		{ "GloryKill", GD.Load<AudioStream>($"{PATH}//glory_kill.wav") },
	};

	/// <inheritdoc/>
	protected override Dictionary<string, AudioStream> FootstepSfxList { get; set; } = new()
	{
		{ "Blood", GD.Load<AudioStream>($"{PATH}//blood.mp3") }
	};
}
