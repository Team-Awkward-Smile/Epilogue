using Godot;

namespace Epilogue.props.camera;
/// <summary>
///		Script for the Camera. Follows a target
/// </summary>
public partial class Camera : Camera2D
{
	private Node2D _cameraTarget;

	public override void _Ready()
	{
		// TODO: Create a method to set the target at runtime
		_cameraTarget = GetNode<Node2D>("../Bob/CameraAnchor");
	}

	public override void _PhysicsProcess(double delta)
	{
		Position = _cameraTarget.GlobalPosition;
	}
}
