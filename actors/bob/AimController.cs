using Godot;
using Godot.Collections;
using System;

namespace Epilogue.actors.hestmor;
public partial class AimController : Node2D
{
	private Dictionary<int, int> aimAngles = new()
	{
		{ 0, -135 }, { 1, -180 }, { 2, -225 }, { 4, -90 }, { 5, 0 }, { 6, 90 }, { 8, -45 }, { 9, 0 }, { 10, 45 }
	};

	public override void _Process(double delta)
	{
		var mousePosition = GetViewport().GetMousePosition();
		var screenSize = DisplayServer.WindowGetSize();
		var flagX = (int) (mousePosition.X / (screenSize.X / 3));
		var flagY = (int) (mousePosition.Y / (screenSize.Y / 3));
		var quadrant = flagX << 2 | flagY;

		if(aimAngles.TryGetValue(quadrant, out var angle))
		{
			GetNode<Sprite2D>("../AimArrow").RotationDegrees = angle;
		}
	}
}
