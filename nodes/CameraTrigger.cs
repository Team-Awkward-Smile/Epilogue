using Epilogue.props.camera;
using Godot;
using System;

namespace Epilogue.nodes;
/// <summary>
///     Area2D Node used to change the Camera's zoom, position, boundaries, etc.
/// </summary>
[GlobalClass]
public partial class CameraTrigger : Area2D
{
    [ExportGroup("Debug"), Export] private bool _hideDuringGame = false;
    [ExportGroup("Properties"), Export] private float _cameraZoom = 3f;
    [Export] private float _zoomDeltaTime = 0.2f;
    [Export] private CameraBlockDirection _cameraBlockDirection;

    private bool _showInfoBackup = true;

    public override void _EnterTree()
    {
        ZIndex = 100;
    }

    public override void _Ready()
    {
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

        var camera = GetViewport().GetCamera2D() as Camera;
        
        camera.SetZoomWithSmoothing(_cameraZoom, _zoomDeltaTime);

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
            camera.LimitLeft = (int) (collision.GlobalPosition.X - horizontal);
        }

        if(_cameraBlockDirection.HasFlag(CameraBlockDirection.Right))
        {
            camera.LimitRight = (int) (collision.GlobalPosition.X + horizontal);
        }

        if(_cameraBlockDirection.HasFlag(CameraBlockDirection.Top))
        {
            camera.LimitTop = (int) (collision.GlobalPosition.Y - vertical);
        }

        if(_cameraBlockDirection.HasFlag(CameraBlockDirection.Bottom))
        {
            camera.LimitBottom = (int) (collision.GlobalPosition.Y + vertical);
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


