using Epilogue.props.camera;
using Godot;
using System.Collections.Generic;

namespace Epilogue.nodes;
/// <summary>
///     Debug Node that draws information useful during level design, like jump distance, camera size, etc.
/// </summary>
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

    [Export] private Vector2 CameraZoom 
    {
        get => _cameraZoom;
        set
        {
            _cameraZoom = value;

            if(_camera is not null)
            {
                _camera.Zoom = _cameraZoom;
            }
        }
    }

    private int _numberofPoints = 60;
    private DrawDirectionEnum _drawDirection;
    private float _standingJumpVSpeed;
    private float _lowJumpHSpeed;
    private float _lowJumpVSpeed;
    private float _longJumpHSpeed;
    private float _longJumpVSpeed;
    private Camera _camera;
    private Vector2 _cameraZoom = new(3f, 3f);

    /// <inheritdoc/>
    public override void _EnterTree()
    {
        if(!Engine.IsEditorHint())
        {
            QueueFree();
        }

        if(_camera is null)
        {
            _camera = new Camera()
            {
                Zoom = _cameraZoom
            };

            AddChild(_camera, false, InternalMode.Front);
        }
    }

    /// <inheritdoc/>
	public override void _Draw()
	{
        var player = GD.Load<PackedScene>("res://actors/bob/bob.tscn").Instantiate();
        var jumpNode = player.GetNode("StateMachine");

        _standingJumpVSpeed = jumpNode.Get("_standingJumpVerticalSpeed").AsSingle();
        _lowJumpHSpeed = jumpNode.Get("_lowJumpHorizontalSpeed").AsSingle();
        _lowJumpVSpeed = jumpNode.Get("_lowJumpVerticalSpeed").AsSingle();
        _longJumpHSpeed = jumpNode.Get("_longJumpHorizontalSpeed").AsSingle();
        _longJumpVSpeed = jumpNode.Get("_longJumpVerticalSpeed").AsSingle();

		var a_long = Mathf.Atan2(-_standingJumpVSpeed, _longJumpHSpeed);
        var a_low = Mathf.Atan2(-_standingJumpVSpeed, _lowJumpHSpeed);

        var v_long = Mathf.Sqrt(Mathf.Pow(_longJumpHSpeed, 2) + Mathf.Pow(-_longJumpVSpeed, 2));
		var v_low = Mathf.Sqrt(Mathf.Pow(_lowJumpHSpeed, 2) + Mathf.Pow(-_lowJumpVSpeed, 2));

        var points_long = new List<Vector2>() { Vector2.Zero };
        var points_low = new List<Vector2>() { Vector2.Zero };

		for(var i = 0; i < NumberOfPoints; i++)
		{
			var x = i;
			var y = x * Mathf.Tan(a_long) - (980f * Mathf.Pow(x, 2) / (2 * Mathf.Pow(v_long, 2) * Mathf.Pow(Mathf.Cos(a_long), 2)));
            
            points_long.Add(new Vector2(x * (DrawDirection == DrawDirectionEnum.Left ? -1 : 1), -y));

            // -----------------------------

            y = x * Mathf.Tan(a_low) - (980f * Mathf.Pow(x, 2) / (2 * Mathf.Pow(v_low, 2) * Mathf.Pow(Mathf.Cos(a_low), 2)));
            
            points_low.Add(new Vector2(x * (DrawDirection == DrawDirectionEnum.Left ? -1 : 1), -y));
		}

        var h = Mathf.Pow(-_standingJumpVSpeed, 2) / (2 * 980f);

        DrawLine(Vector2.Zero, new Vector2(0f, -h), new Color(1f, 0.923f, 0f));
        DrawPolyline(points_long.ToArray(), new Color(1f, 0f, 0f));
        DrawPolyline(points_low.ToArray(), new Color(1f, 0.6f, 0f));
	}
}
