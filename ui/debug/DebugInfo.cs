using Godot;

namespace Epilogue.UI.debug;
/// <summary>
///		Prints various messages on-screen to help with debugging
/// </summary>
public partial class DebugInfo : Node
{
	[Export] private bool _playerSpeed;
	[Export] private bool _branchName = true;
	[Export] private bool _aimWheel = false;
	[Export] private bool _inputType = false;

	private Node2D _wheelParent;

	/// <inheritdoc/>
	public override void _Ready()
	{
		if(_aimWheel)
		{
			GetViewport().SizeChanged += SetAimWheelLines;
			SetAimWheelLines();
		}

		var hContainer = GetNode<HBoxContainer>("HBoxContainer");

		if(_playerSpeed)
		{
			hContainer.AddChild(new PlayerVelocity());
		}

		if(_branchName)
		{
			hContainer.AddChild(new BranchName());
		}

		if(_inputType)
		{
			hContainer.AddChild(new InputType());
		}
	}

	private void SetAimWheelLines()
	{
		_wheelParent?.QueueFree();

		var screenSize = DisplayServer.WindowGetSize();

		_wheelParent = new Node2D
		{
			Position = screenSize / 2,
			RotationDegrees = 22.5f
		};

		AddChild(_wheelParent);

		for(var i = 0; i <= 8; i++)
		{
			var p = new Vector2(3000, 0).Rotated(Mathf.DegToRad(45f * i));

			var line = new Line2D()
			{
				Points = new[] { Vector2.Zero, p },
				Width = 1f
			};

			_wheelParent.AddChild(line);
		}
	}
}
