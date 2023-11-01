using Epilogue.props.camera;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Epilogue.nodes;
/// <summary>
///     Area2D Node used to change the Camera's zoom, position, boundaries, etc.
/// </summary>
[GlobalClass, Tool]
public partial class CameraTrigger : Area2D
{
    [ExportGroup("Debug"), Export] private bool _hideDuringGame = false;
    [ExportGroup("Properties"), Export] private float _cameraZoom = 3f;
    [Export] private float _zoomDeltaTime = 0.2f;
    [Export] private CameraBlockDirection _cameraBlockDirection
    {
        get => _cameraBlock_bk;
        set 
        {
            _cameraBlock_bk = value;
            QueueRedraw();
        }
    }

    private bool _showInfoBackup = true;
    private Camera _camera;
    private CameraBlockDirection _cameraBlock_bk;
    private CollisionShape2D _collision;

    public override void _EnterTree()
    {
        if(GetChildCount() > 0)
        {
            _collision = GetChildren().OfType<CollisionShape2D>().First();
            _collision.DebugColor = new Color(246f / 255, 217f / 255, 30f / 255, 107f / 255);
        }

        ZIndex = 100;
    }

    public override void _Ready()
    {
        if(Engine.IsEditorHint())
        {
            return;
        }

        if(_hideDuringGame)
        {
            Modulate = new Color(0f, 0f, 0f, 0f);
        }

        BodyEntered += OnBodyEntered;
    }

    private void OnBodyEntered(Node2D body)
    {
        if(body is not Player)
        {
            return;
        }

        _camera = GetViewport().GetCamera2D() as Camera;
        _camera.SetZoomWithSmoothing(_cameraZoom, _zoomDeltaTime);

        if(_cameraBlockDirection == 0)
        {
            return;
        }

        var collision = GetChild(0) as CollisionShape2D;
        var bounds = collision.Shape.GetRect();
        var horizontal = bounds.Size.X / 2;
        var vertical = bounds.Size.Y / 2;
        
        if(_cameraBlockDirection.HasFlag(CameraBlockDirection.Left))
        {
            _camera.LimitLeft = (int) (collision.GlobalPosition.X - horizontal);
        }

        if(_cameraBlockDirection.HasFlag(CameraBlockDirection.Right))
        {
            _camera.LimitRight = (int) (collision.GlobalPosition.X + horizontal);
        }

        if(_cameraBlockDirection.HasFlag(CameraBlockDirection.Top))
        {
            _camera.LimitTop = (int) (collision.GlobalPosition.Y - vertical);
        }

        if(_cameraBlockDirection.HasFlag(CameraBlockDirection.Bottom))
        {
            _camera.LimitBottom = (int) (collision.GlobalPosition.Y + vertical);
        }
    }

    public override void _Draw()
    {
        if(_cameraBlockDirection == 0)
        {
            return;
        }

        var points = new List<Vector2>();
        var bounds = _collision.Shape.GetRect();

        if(_cameraBlockDirection.HasFlag(CameraBlockDirection.Left))
        {
            points.Add(new(_collision.Position.X - (bounds.Size.X / 2), _collision.Position.Y - (bounds.Size.Y / 2)));
            points.Add(new (_collision.Position.X - (bounds.Size.X / 2), _collision.Position.Y + (bounds.Size.Y / 2)));
        }

        if(_cameraBlockDirection.HasFlag(CameraBlockDirection.Right))
        {
            points.Add(new(_collision.Position.X + (bounds.Size.X / 2), _collision.Position.Y - (bounds.Size.Y / 2)));
            points.Add(new (_collision.Position.X + (bounds.Size.X / 2), _collision.Position.Y + (bounds.Size.Y / 2)));
        }

        if(_cameraBlockDirection.HasFlag(CameraBlockDirection.Top))
        {
            points.Add(new(_collision.Position.X - (bounds.Size.X / 2), _collision.Position.Y - (bounds.Size.Y / 2)));
            points.Add(new (_collision.Position.X + (bounds.Size.X / 2), _collision.Position.Y - (bounds.Size.Y / 2)));
        }

        if(_cameraBlockDirection.HasFlag(CameraBlockDirection.Bottom))
        {
            points.Add(new(_collision.Position.X - (bounds.Size.X / 2), _collision.Position.Y + (bounds.Size.Y / 2)));
            points.Add(new (_collision.Position.X + (bounds.Size.X / 2), _collision.Position.Y + (bounds.Size.Y / 2)));
        }

        if(points.Count > 0)
        {
            DrawMultiline(points.ToArray(), new Color(1f, 0f, 0f), 20f);
        }
    }

    [Flags]
    private enum CameraBlockDirection
    {
        Left = 1 << 1,
        Top = 1 << 2,
        Right = 1 << 3,
        Bottom = 1 << 4
    }
}


