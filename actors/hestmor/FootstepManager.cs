using Epilogue.Extensions;
using Epilogue.Global.Enums;
using Epilogue.Nodes;
using Godot;
using System;

namespace Epilogue.Actors.Hestmor;
/// <summary>
///		Node used to control the SFXs played when an Actor walks around the map
/// </summary>
public partial class FootstepManager : Node2D
{
	/// <summary>
	///		Signal emitted whenever an animation track wants to play an SFX for footsteps
	/// </summary>
	/// <param name="tileType"></param>
	[Signal] public delegate void PlayerSteppedOnTileEventHandler(TileType tileType);

	private Level _level;

	/// <inheritdoc/>
	public override void _Ready()
	{
		_level = GetTree().GetLevel();
	}

	/// <summary>
	///		Plays a random footstep SFX based on the Tile this Node is currently touching
	/// </summary>
	public void PlayRandomFootstepSfx()
	{
		var tileType = _level.GetTileDataAtPosition(GlobalPosition).GetCustomData("ground_type").AsInt32();

		EmitSignal(SignalName.PlayerSteppedOnTile, tileType);
	}
}
