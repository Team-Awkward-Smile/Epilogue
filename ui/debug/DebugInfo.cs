using Godot;
using System.Linq;

namespace Epilogue.ui.debug;
public partial class DebugInfo : Node
{
	[Export] private bool _playerSpeed;
	[Export] private bool _branchName = true;
	[Export] private bool _aimQuadrants = false;
	[Export] private bool _inputType = false;

	public override void _Ready()
	{
		if(_aimQuadrants)
		{
			GetViewport().SizeChanged += SetAimQuadrantLines;
			SetAimQuadrantLines();
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

	private void SetAimQuadrantLines()
	{
		foreach(var c in GetChildren().OfType<Line2D>().ToList())
		{
			c.QueueFree();
		}

		var screenSize = DisplayServer.WindowGetSize();

		for(var i = 1; i <= 2; i++)
		{
			var line = new Line2D()
			{
				Width = 1
			};

			line.AddPoint(new Vector2(screenSize.X / 3 * i, 0f));
			line.AddPoint(new Vector2(screenSize.X / 3 * i, screenSize.Y));

			AddChild(line);
		}

		for(var i = 1; i <= 2; i++)
		{
			var line = new Line2D()
			{
				Width = 1
			};

			line.AddPoint(new Vector2(0f, screenSize.Y / 3 * i));
			line.AddPoint(new Vector2(screenSize.X, screenSize.Y / 3 * i));

			AddChild(line);
		}
	}
}
