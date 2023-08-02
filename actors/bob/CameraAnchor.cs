using Epilogue.nodes;
using Godot;

namespace Epilogue.actors.hestmor;
/// <summary>
///		Node responsible for making the Camera follow a target smoothly
/// </summary>
public partial class CameraAnchor : Node2D
{
	Actor _actor;

	/// <inheritdoc/>
	public override void _Ready()
	{
		// TODO: replace the hard-coded Actor for a generic target
		_actor = (Actor) Owner;
	}

	/// <inheritdoc/>
	public override void _PhysicsProcess(double delta)
	{
		var x = Mathf.Lerp(GlobalPosition.X, _actor.GlobalPosition.X, 0.1f);
		var y = Mathf.Lerp(GlobalPosition.Y, _actor.GlobalPosition.Y, 0.1f);

		GlobalPosition = new Vector2(x, y);
	}
}
