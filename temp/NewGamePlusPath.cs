using Epilogue.global.singletons;
using Godot;
using System;

public partial class NewGamePlusPath : TileMap
{
	public override void _Ready()
	{
		if(Settings.GameCycle == Epilogue.global.enums.GameCycle.NewGamePlus)
		{
			QueueFree();
		}
	}
}
