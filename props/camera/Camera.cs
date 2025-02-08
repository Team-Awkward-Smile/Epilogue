using Epilogue.Const;
using Epilogue.Extensions;
using Godot;

namespace Epilogue.Props.camera;
/// <summary>
///		Camera Nodes with functionalities required by this project
/// </summary>
public partial class Camera : Camera2D
{


	[Export] public bool ZoomOut = false;

	private Node2D _cameraTarget;

	public override void _Ready()
	{
		base._Ready();
		
		if (ZoomOut)
		{
			Zoom = new Vector2(1f, 1f);
		}
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
		if (_cameraTarget is null)
		{
			return;
		}

		Position = _cameraTarget.GlobalPosition;
	}

	public void LimitCameraToCurrentTileMap()
	{
		var tileMap = GetTree().GetLevel().TileMap;
		var bounds = tileMap.GetUsedRect();

		LimitLeft = bounds.Position.X * Constants.MAP_TILE_SIZE;
		LimitTop = bounds.Position.Y * Constants.MAP_TILE_SIZE;
		LimitRight = bounds.End.X * Constants.MAP_TILE_SIZE;
		LimitBottom = bounds.End.Y * Constants.MAP_TILE_SIZE;
	}
}