using Godot;

namespace Epilogue.props.camera;
public partial class Camera : Camera2D
{
	private Node2D _cameraTarget;

	public override void _Ready()
	{
		_cameraTarget = GetNode<Node2D>("../../Bob/CameraAnchor");
	}

	public override void _PhysicsProcess(double delta)
	{
		Position = _cameraTarget.GlobalPosition;
	}
}
