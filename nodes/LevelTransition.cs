using Epilogue.nodes;
using Godot;
using System;
using System.Linq;

[GlobalClass, Tool]
public partial class LevelTransition : Area2D
{
    [Export] private Levels _levelToLoad;

    public override void _EnterTree()
    {
        if(GetChildCount() > 0)
        {
            GetChildren().OfType<CollisionShape2D>().First().DebugColor = new Color(45f / 255, 239f / 255, 103f / 255, 107f / 255);
        }
    }

    public override void _Ready()
    {
        BodyEntered += OnBodyEntered;
    }

    private void OnBodyEntered(Node2D body)
    {
        if(body is not Player)
        {
            return;
        }

        GetTree().CallDeferred("change_scene_to_file", _levelToLoad switch 
        {
            Levels.TestScene => "res://levels/level_select/level_select.tscn",
            Levels.HestmorsNest => "res://levels/level_1/Nest.cs",
            _ => throw new Exception($"Level [{_levelToLoad}] undefined")
        });
    }
}
