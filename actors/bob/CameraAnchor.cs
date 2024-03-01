using Epilogue.Const;
using Epilogue.Nodes;
using Godot;

namespace Epilogue.Actors.Hestmor;
/// <summary>
///		Node responsible for making the Camera follow a target smoothly
/// </summary>
public partial class CameraAnchor : Node2D
{
	/// <summary>
	///		Defines if the Anchor will automatically follow the player around
	/// </summary>
	[Export] public bool FollowPlayer { get; set; } = true;

	private Node2D _target;
	private Vector2 _targetOffset;
	private Vector2 _originalPosition;

	/// <inheritdoc/>
	public override void _Ready()
	{
		_target = (Player)Owner;
		_targetOffset = _target.GlobalPosition - new Vector2(0f, Constants.PLAYER_HEIGHT / 2);
	}

	/// <inheritdoc/>
	public override void _PhysicsProcess(double delta)
	{
		if (!FollowPlayer)
		{
			return;
		}

		var x = Mathf.Lerp(Position.X, _targetOffset.X, 0.1f);
		var y = Mathf.Lerp(Position.Y, _targetOffset.Y, 0.1f);

		Position = new Vector2(x, y);
	}

	public void ResetPosition()
	{
		Position = _targetOffset;
	}
}
