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
		{ "KneeSlide", GD.Load<AudioStream>($"{PATH}//KneeSlide_SFX.wav") },
		{ "Slide", GD.Load<AudioStream>($"{PATH}//Slide_SFX.wav") },
		{ "Jump", GD.Load<AudioStream>($"{PATH}//JumpingStart_SFX.wav") },
		{ "Land", GD.Load<AudioStream>($"{PATH}//JumpingLand_SFX.wav") },
	 
		{ "SlashAttack1", GD.Load<AudioStream>($"{PATH}//generic/slash/SlashAttack1.wav") },
		{ "SlashAttack2", GD.Load<AudioStream>($"{PATH}//generic/slash/SlashAttack2.wav") },
		{ "SlashAttack3", GD.Load<AudioStream>($"{PATH}//generic/slash/SlashAttack3.wav") },

		{ "GloryKill", GD.Load<AudioStream>($"{PATH}//glory_kill.wav") },
		{ "Crouch_01", GD.Load<AudioStream>($"{PATH}//generic//crouch_1.wav") },
		{ "Crouch_02", GD.Load<AudioStream>($"{PATH}//generic//crouch_2.wav") },
		{ "Idle", GD.Load<AudioStream>($"{PATH}//HestmorIdleBreathingLoop.mp3") },
		{ "Sleeping", GD.Load<AudioStream>($"{PATH}//generic//sleep//SleepBreathing.mp3") },
		{ "SleepStart", GD.Load<AudioStream>($"{PATH}//generic//sleep//SleepingCrouch.mp3") },

		{ "UpperCut_01", GD.Load<AudioStream>($"{PATH}//generic//upper_cut/UpperCut_01.wav") },
		{ "UpperCut_02", GD.Load<AudioStream>($"{PATH}//generic//upper_cut/UpperCut_02.wav") },
		{ "UpperCut_03", GD.Load<AudioStream>($"{PATH}//generic//upper_cut/UpperCut_03.wav") },


		{ "DeathBreath_01", GD.Load<AudioStream>($"{PATH}//generic//death_breath//DeathBreath1.mp3") },
		{ "DeathBreath_02", GD.Load<AudioStream>($"{PATH}//generic//death_breath//DeathBreath2.mp3") },
		{ "DeathBreath_03", GD.Load<AudioStream>($"{PATH}//generic//death_breath//DeathBreath3.mp3") },
		{ "DeathBreath_04", GD.Load<AudioStream>($"{PATH}//generic//death_breath//DeathBreath4.mp3") },
		{ "DeathBreath_05", GD.Load<AudioStream>($"{PATH}//generic//death_breath//DeathBreath5.mp3") },
		{ "DeathBreath_06", GD.Load<AudioStream>($"{PATH}//generic//death_breath//DeathBreath6.mp3") },

		{ "RunAttack", GD.Load<AudioStream>($"{PATH}//generic//run_attack//RunAttack.wav") },
		
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

		{ "KneeSlideRock_01", GD.Load<AudioStream>($"{PATH}//collision//kneeslide//rock//rkneeslide_01.wav") },
		{ "KneeSlideFlesh_01", GD.Load<AudioStream>($"{PATH}//collision//kneeslide//flesh//fkneeslide_01.wav") },

		{ "GrabRock_01", GD.Load<AudioStream>($"{PATH}//collision//ledge//rock//rgrab_ledge_01.wav") },
		{ "GrabRock_02", GD.Load<AudioStream>($"{PATH}//collision//ledge//rock//rgrab_ledge_02.wav") },
		{ "GrabRock_03", GD.Load<AudioStream>($"{PATH}//collision//ledge//rock//rgrab_ledge_03.wav") },

		{ "ClimbRock_01", GD.Load<AudioStream>($"{PATH}//collision//ledge//rock//rclimbing_01.wav") },
		{ "ClimbRock_02", GD.Load<AudioStream>($"{PATH}//collision//ledge//rock//rclimbing_02.wav") },
		{ "ClimbRock_03", GD.Load<AudioStream>($"{PATH}//collision//ledge//rock//rclimbing_03.wav") },
		{ "ClimbRock_04", GD.Load<AudioStream>($"{PATH}//collision//ledge//rock//rclimbing_04.wav") },

		{ "ClimbFlesh_01", GD.Load<AudioStream>($"{PATH}//collision//vault//VaultFlesh_01.mp3") },
		{ "ClimbFlesh_02", GD.Load<AudioStream>($"{PATH}//collision//vault//VaultFlesh_02.mp3") },
		{ "ClimbFlesh_03", GD.Load<AudioStream>($"{PATH}//collision//vault//VaultFlesh_03.mp3") },
		{ "ClimbFlesh_04", GD.Load<AudioStream>($"{PATH}//collision//vault//VaultFlesh_04.mp3") },

		{ "FallGrabFlesh_01", GD.Load<AudioStream>($"{PATH}//collision//ledge//flesh//FleshLedgeFallGrab1.wav") },
		{ "FallGrabFlesh_02", GD.Load<AudioStream>($"{PATH}//collision//ledge//flesh//FleshLedgeFallGrab2.wav") },
		{ "FallGrabFlesh_03", GD.Load<AudioStream>($"{PATH}//collision//ledge//flesh//FleshLedgeFallGrab3.wav") },
				
		{ "ScratchRock_01", GD.Load<AudioStream>($"{PATH}//collision//melee//rock//scratch.wav") },
		{ "ScratchRock_02", GD.Load<AudioStream>($"{PATH}//collision//melee//rock//scratch_01.wav") },
		{ "ScratchRock_03", GD.Load<AudioStream>($"{PATH}//collision//melee//rock//scratch_02.wav") },
		{ "ScratchRock_04", GD.Load<AudioStream>($"{PATH}//collision//melee//rock//scratch_short.wav") },
		{ "ScratchRock_05", GD.Load<AudioStream>($"{PATH}//collision//melee//rock//scratch_short_01.wav") },
		{ "ScratchRock_06", GD.Load<AudioStream>($"{PATH}//collision//melee//rock//scratch_short_02.wav") },

		{ "LandFlesh_01", GD.Load<AudioStream>($"{PATH}//collision//land//flesh//FleshLand1.wav") },
		{ "LandFlesh_02", GD.Load<AudioStream>($"{PATH}//collision//land//flesh//FleshLand2.wav") },
		{ "LandFlesh_03", GD.Load<AudioStream>($"{PATH}//collision//land//flesh//FleshLand3.wav") },
		{ "LandFlesh_04", GD.Load<AudioStream>($"{PATH}//collision//land//flesh//FleshLand4.wav") },

		{ "RollFlesh_01", GD.Load<AudioStream>($"{PATH}//collision//roll//flesh//FleshRoll1.wav") },
		{ "RollFlesh_02", GD.Load<AudioStream>($"{PATH}//collision//roll//flesh//FleshRoll2.wav") },
		{ "RollFlesh_03", GD.Load<AudioStream>($"{PATH}//collision//roll//flesh//FleshRoll3.wav") },
		{ "RollFlesh_04", GD.Load<AudioStream>($"{PATH}//collision//roll//flesh//FleshRoll4.wav") },
		

	};

	/// <inheritdoc/>
	public override void _Ready()
	{
		base._Ready();

		GetNode<FootstepManager>("FootstepManager").PlayerSteppedOnTile += (TileType tileType) => PlayRandomFootstepSfx($"Step{tileType}");
		GetNode<FootstepManager>("FootstepManager").PlayerCollisionOnTile += (string prefix, TileType tileType) => PlayRandomCollisionSfx($"{prefix}{tileType}");
	}
}
