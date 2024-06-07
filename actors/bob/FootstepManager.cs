using Epilogue.Extensions;
using Epilogue.Global.Enums;
using Epilogue.Nodes;
using Godot;
using System;

public partial class FootstepManager : Node2D
{
	[Signal] public delegate void PlayerSteppedOnTileEventHandler(TileType tileType);

	private Level _level;

	public override void _Ready()
	{
		_level = GetTree().GetLevel();
	}

	public void PlayRandomFootstepSfx()
	{
		var tileType = _level.GetTileDataAtPosition(GlobalPosition).GetCustomData("ground_type").AsInt32();

		EmitSignal(SignalName.PlayerSteppedOnTile, tileType);
	}
}
