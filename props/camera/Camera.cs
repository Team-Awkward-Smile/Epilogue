using Godot;

namespace Epilogue.props.camera;
/// <summary>
///		Camera Nodes with functionalities required by this project
/// </summary>
public partial class Camera : Camera2D
{
	private Node2D _cameraTarget;

	/// <inheritdoc/>
	public override void _Ready()
	{
		_cameraTarget = GetNode<Node2D>("../../Bob/CameraAnchor");
	}

	/// <inheritdoc/>
	public override void _PhysicsProcess(double delta)
	{
		Position = _cameraTarget.GlobalPosition;
	}
}
