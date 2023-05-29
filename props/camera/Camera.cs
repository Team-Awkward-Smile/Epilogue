using Godot;

public partial class Camera : Camera2D
{
	private Node2D _cameraTarget;

	public override void _Ready()
	{
		_cameraTarget = GetNode<CharacterBody2D>("../Bob");
	}

	public override void _PhysicsProcess(double delta)
	{
		Position = _cameraTarget.Position;
	}
}
