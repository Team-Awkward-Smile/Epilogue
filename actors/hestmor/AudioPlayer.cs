using Epilogue.Global.Enums;
using Epilogue.Nodes;
using Godot;
using Godot.Collections;

namespace Epilogue.Actors.Hestmor;
/// <summary>
///		Collection of sound effects used by Hestmor
/// </summary>
public partial class AudioPlayer : ActorAudioPlayer
{
	private static readonly string PATH = @"res://actors/hestmor/sfx";

	/// <inheritdoc/>
	protected override Dictionary<string, AudioStream> GenericSfxList { get; set; } = new()
	{
		{ "Slide", GD.Load<AudioStream>($"{PATH}//Slide_SFX.wav") },
		{ "Jump", GD.Load<AudioStream>($"{PATH}//JumpingStart_SFX.wav") },
		{ "Land", GD.Load<AudioStream>($"{PATH}//JumpingLand_SFX.wav") },
		{ "Melee", GD.Load<AudioStream>($"{PATH}//SwipingAttack_SFX.wav") },
		{ "GloryKill", GD.Load<AudioStream>($"{PATH}//glory_kill.wav") },
		{ "Crouch1", GD.Load<AudioStream>($"{PATH}//generic//crouch_1.wav") },
		{ "Crouch2", GD.Load<AudioStream>($"{PATH}//generic//crouch_2.wav") },
	};

    /// <inheritdoc/>
	protected override Dictionary<string, AudioStream> FootstepSfxList { get; set; } = new()
	{
		{ "StepRock_01", GD.Load<AudioStream>($"{PATH}//footsteps//rock//rsteps_01.wav") },
		{ "StepRock_02", GD.Load<AudioStream>($"{PATH}//footsteps//rock//rsteps_02.wav") },
		{ "StepRock_03", GD.Load<AudioStream>($"{PATH}//footsteps//rock//rsteps_03.wav") },
		{ "StepRock_04", GD.Load<AudioStream>($"{PATH}//footsteps//rock//rsteps_04.wav") },
		{ "StepRock_05", GD.Load<AudioStream>($"{PATH}//footsteps//rock//rsteps_05.wav") },
		{ "StepRock_06", GD.Load<AudioStream>($"{PATH}//footsteps//rock//rsteps_06.wav") },
		{ "StepRock_07", GD.Load<AudioStream>($"{PATH}//footsteps//rock//rsteps_07.wav") },
		{ "StepRock_08", GD.Load<AudioStream>($"{PATH}//footsteps//rock//rsteps_08.wav") },
		{ "StepRock_09", GD.Load<AudioStream>($"{PATH}//footsteps//rock//rsteps_09.wav") },

		{ "StepFlesh_01", GD.Load<AudioStream>($"{PATH}//footsteps//flesh//StepFlesh_01.wav") },
		{ "StepFlesh_02", GD.Load<AudioStream>($"{PATH}//footsteps//flesh//StepFlesh_02.wav") },
		{ "StepFlesh_03", GD.Load<AudioStream>($"{PATH}//footsteps//flesh//StepFlesh_03.wav") },
		{ "StepFlesh_04", GD.Load<AudioStream>($"{PATH}//footsteps//flesh//StepFlesh_04.wav") },
		{ "StepFlesh_05", GD.Load<AudioStream>($"{PATH}//footsteps//flesh//StepFlesh_05.wav") },
		{ "StepFlesh_06", GD.Load<AudioStream>($"{PATH}//footsteps//flesh//StepFlesh_06.wav") },
		{ "StepFlesh_07", GD.Load<AudioStream>($"{PATH}//footsteps//flesh//StepFlesh_07.wav") },
		{ "StepFlesh_08", GD.Load<AudioStream>($"{PATH}//footsteps//flesh//StepFlesh_08.wav") },
	};

    /// <inheritdoc/>
    protected override Dictionary<string, AudioStream> CollisionSfxList { get; set; } = new()
	{
		{ "SlideRock_01", GD.Load<AudioStream>($"{PATH}//collision//slide//rock//rslide_01.wav") },

		{ "GrabRock_01", GD.Load<AudioStream>($"{PATH}//collision//ledge//rock//rgrab_ledge_01.wav") },
		{ "GrabRock_02", GD.Load<AudioStream>($"{PATH}//collision//ledge//rock//rgrab_ledge_02.wav") },
		{ "GrabRock_03", GD.Load<AudioStream>($"{PATH}//collision//ledge//rock//rgrab_ledge_03.wav") },

		{ "ClimbRock_01", GD.Load<AudioStream>($"{PATH}//collision//ledge//rock//rclimbing_01.wav") },
		{ "ClimbRock_02", GD.Load<AudioStream>($"{PATH}//collision//ledge//rock//rclimbing_02.wav") },
		{ "ClimbRock_03", GD.Load<AudioStream>($"{PATH}//collision//ledge//rock//rclimbing_03.wav") },
		{ "ClimbRock_04", GD.Load<AudioStream>($"{PATH}//collision//ledge//rock//rclimbing_04.wav") },

		{ "ScratchRock_01", GD.Load<AudioStream>($"{PATH}//collision//melee//rock//scratch.wav") },
		{ "ScratchRock_02", GD.Load<AudioStream>($"{PATH}//collision//melee//rock//scratch_01.wav") },
		{ "ScratchRock_03", GD.Load<AudioStream>($"{PATH}//collision//melee//rock//scratch_02.wav") },
		{ "ScratchRock_04", GD.Load<AudioStream>($"{PATH}//collision//melee//rock//scratch_short.wav") },
		{ "ScratchRock_05", GD.Load<AudioStream>($"{PATH}//collision//melee//rock//scratch_short_01.wav") },
		{ "ScratchRock_06", GD.Load<AudioStream>($"{PATH}//collision//melee//rock//scratch_short_02.wav") },
		
		{ "RollFlesh_01", GD.Load<AudioStream>($"{PATH}//collision//rolling//flesh//FleshRoll1.wav") },
		{ "RollFlesh_02", GD.Load<AudioStream>($"{PATH}//collision//rolling//flesh//FleshRoll2.wav") },
		{ "RollFlesh_03", GD.Load<AudioStream>($"{PATH}//collision//rolling//flesh//FleshRoll3.wav") },
		{ "RollFlesh_04", GD.Load<AudioStream>($"{PATH}//collision//rolling//flesh//FleshRoll4.wav") }
	};

	/// <inheritdoc/>
	public override void _Ready()
    {
        base._Ready();

		GetNode<FootstepManager>("FootstepManager").PlayerSteppedOnTile += (TileType tileType) => PlayRandomFootstepSfx($"Step{tileType}");
		GetNode<FootstepManager>("FootstepManager").PlayerCollisionOnTile += (string prefix, TileType tileType) => PlayRandomCollisionSfx($"{prefix}{tileType}");
    }
}
