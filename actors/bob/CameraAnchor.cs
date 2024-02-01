using Epilogue.Nodes;
using Godot;

namespace Epilogue.actors.hestmor;
/// <summary>
///		Node responsible for making the Camera follow a target smoothly
/// </summary>
public partial class CameraAnchor : Node2D
{
	/// <summary>
	///		Defines if the Anchor will automatically follow the player around
	/// </summary>
	public bool FollowPlayer { get; set; } = true;

    private Node2D _target;

	/// <inheritdoc/>
	public override void _Ready()
	{
		_target = (Player) Owner;
	}

	/// <inheritdoc/>
	public override void _PhysicsProcess(double delta)
	{
		if(!FollowPlayer)
		{
			return;
		}

		var x = Mathf.Lerp(GlobalPosition.X, _target.GlobalPosition.X, 0.1f);
		var y = Mathf.Lerp(GlobalPosition.Y, _target.GlobalPosition.Y, 0.1f);

		GlobalPosition = new Vector2(x, y);
	}
}
