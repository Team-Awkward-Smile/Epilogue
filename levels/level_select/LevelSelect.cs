using Godot;
using System;
using System.Linq;

namespace Epilogue.ui.level_select;
public partial class LevelSelect : Node
{
	public override void _Ready()
	{
		var i = 1;

        foreach(Button btn in GetNode("MarginContainer/GridContainer").GetChildren().OrderBy(c => c.Name.ToString()))
        {
            var temp = i++;

            btn.Pressed += () => 
            {
                GetTree().ChangeSceneToFile($"res://levels/level_{temp}/level_{temp}.tscn");
            };
        }
	}
}
