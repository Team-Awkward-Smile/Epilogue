using Epilogue.global.enums;
using Godot;

namespace Epilogue.actors.hestmor.aim;
public partial class MouseAim : Node
{
	private Aim _aim;

	public override void _Ready()
	{
		_aim = (Aim) GetParent();
	}

	public override void _Process(double delta)
	{
		var mousePosition = GetViewport().GetMousePosition();
		var screenSize = DisplayServer.WindowGetSize();
		var flagX = AimDirection.None;
		var flagY = AimDirection.None;



		_aim.SetAimDirection(flagX | flagY);
	}
}
