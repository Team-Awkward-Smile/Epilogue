using Godot;

namespace Epilogue.props.camera;
/// <summary>
///		Camera Nodes with functionalities required by this project
/// </summary>
public partial class Camera : Camera2D
{
	private Node2D _cameraTarget;

    public override void _EnterTree()
    {
		LimitSmoothed = true;
		PositionSmoothingEnabled = true;
		PositionSmoothingSpeed = 10f;
    }

    /// <summary>
    ///		Sets a new target for this Camera to follow
    /// </summary>
    /// <param name="cameraTarget">The Node2D that will be followed by the Camera</param>
    public void SetCameraTarget(Node2D cameraTarget)
	{
		_cameraTarget = cameraTarget;
	}

	/// <inheritdoc/>
	public override void _PhysicsProcess(double delta)
	{
		if(_cameraTarget is null)
		{
			return;
		}

		Position = _cameraTarget.GlobalPosition;
	}

	public void SetZoomWithSmoothing(float zoomValue, float time)
	{
		GetTree().CreateTween().TweenProperty(this, "zoom", new Vector2(zoomValue, zoomValue), time);
	}

	public void ResetZoom()
	{
		Zoom = new Vector2(3f, 3f);
	}

	public void ResetLimits()
	{
		LimitLeft = -10000000;
		LimitRight = 10000000;
		LimitTop = -10000000;
		LimitBottom = 10000000;
	}
}
