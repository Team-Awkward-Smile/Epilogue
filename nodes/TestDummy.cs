using Godot;
using System;
using System.Collections.Generic;

namespace Epilogue.nodes;
[GlobalClass, Tool]
public partial class TestDummy : Node2D
{
    private enum DrawDirectionEnum
    {
        Left,
        Right
    }

    [Export] private DrawDirectionEnum DrawDirection
    {
        get => _drawDirection;
        set 
        {
            _drawDirection = value;
            QueueRedraw();
        }
    }

    [Export] private int NumberOfPoints
    {
        get => _numberofPoints;
        set 
        {
            _numberofPoints = value;
            QueueRedraw();
        }
    }

    [Export] private bool _destroyOnLoad;

    private int _numberofPoints = 60;
    private DrawDirectionEnum _drawDirection;
    private float _verticalJumpSpeed;
    private float _lowJumpSpeed;
    private float _longJumpSpeed;

	public override void _Ready()
	{
		if(_destroyOnLoad && !Engine.IsEditorHint())
        {
            QueueFree();
        }
	}

	public override void _Draw()
	{
        var player = GD.Load<PackedScene>("res://actors/bob/bob.tscn").Instantiate();
        var jumpNode = player.GetNode("StateMachine/Jump");

        _verticalJumpSpeed = jumpNode.Get("_jumpSpeed").AsSingle();
        _lowJumpSpeed = jumpNode.Get("_lowJumpHorizontalSpeed").AsSingle();
        _longJumpSpeed = jumpNode.Get("_longJumpHorizontalSpeed").AsSingle();

		var a_long = Mathf.Atan2(-_verticalJumpSpeed, _longJumpSpeed);
        var a_low = Mathf.Atan2(-_verticalJumpSpeed, _lowJumpSpeed);

        var v_long = Mathf.Sqrt((Mathf.Pow(_longJumpSpeed, 2)) + (Mathf.Pow(-_verticalJumpSpeed, 2)));
		var v_low = Mathf.Sqrt((Mathf.Pow(_lowJumpSpeed, 2)) + (Mathf.Pow(-_verticalJumpSpeed, 2)));

        var points_long = new List<Vector2>() { Vector2.Zero };
        var points_low = new List<Vector2>() { Vector2.Zero };

		for(var i = 0; i < NumberOfPoints; i++)
		{
			var x = i;
			var y = x * Mathf.Tan(a_long) - ((980f * Mathf.Pow(x, 2)) / (2 * Mathf.Pow(v_long, 2) * Mathf.Pow(Mathf.Cos(a_long), 2)));
            
            points_long.Add(new Vector2(x * (DrawDirection == DrawDirectionEnum.Left ? -1 : 1), -y));

            // -----------------------------

            y = x * Mathf.Tan(a_low) - ((980f * Mathf.Pow(x, 2)) / (2 * Mathf.Pow(v_low, 2) * Mathf.Pow(Mathf.Cos(a_low), 2)));
            
            points_low.Add(new Vector2(x * (DrawDirection == DrawDirectionEnum.Left ? -1 : 1), -y));
		}

        var h = Mathf.Pow(-_verticalJumpSpeed, 2) / (2 * 980f);

        DrawLine(Vector2.Zero, new Vector2(0f, -h), new Color(1f, 0.923f, 0f));
        DrawPolyline(points_long.ToArray(), new Color(1f, 0f, 0f));
        DrawPolyline(points_low.ToArray(), new Color(1f, 0.6f, 0f));
	}
}
