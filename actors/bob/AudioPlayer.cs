using Epilogue.nodes;
using Godot;
using Godot.Collections;

namespace Epilogue.actors.hestmor;
public partial class AudioPlayer : AudioPlayerBase
{
	private static string path = @"res://actors/bob/sfx";

	protected override Dictionary<string, AudioStream> GenericSfxList { get; set; } = new()
	{
		{ "Slide", GD.Load<AudioStream>($"{path}//Slide_SFX.wav") },
		{ "Jump", GD.Load<AudioStream>($"{path}//JumpingStart_SFX.wav") },
		{ "Land", GD.Load<AudioStream>($"{path}//JumpingLand_SFX.wav") },
	};
}
