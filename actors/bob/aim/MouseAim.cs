using Epilogue.Global.Enums;
using Epilogue.Nodes;
using Godot;

namespace Epilogue.actors.hestmor.aim;
/// <summary>
///		Node responsible for handling aiming inputs from a mouse
/// </summary>
public partial class MouseAim : Node
{
	private Actor _actor;
	private Aim _aim;

	/// <inheritdoc/>
	public override void _Ready()
	{
		_actor = (Actor) Owner;
		_aim = (Aim) GetParent();
	}

	/// <inheritdoc/>
	public override void _Process(double delta)
	{
		var screenSize = DisplayServer.WindowGetSize();
		var mousePosition = (GetViewport().GetMousePosition() - (screenSize / 2)) * new Vector2(1f, -1f);
		var angle = Mathf.RadToDeg(Mathf.Atan2(mousePosition.Y, mousePosition.X)) + 22.5f;
		var wheelArea = Mathf.Floor(angle / 45f);

		var flagX = AimDirection.None;
		var flagY = angle >= 0 ? AimDirection.Up : AimDirection.Down;

		switch(Mathf.Abs(wheelArea))
		{
			case 0:
				flagY = AimDirection.None;
				flagX = AimDirection.Right;
				break;

			case 1:
				flagX = AimDirection.Right;
				break;

			case 2:
				flagX = AimDirection.None;
				break;

			case 3:
				flagX = AimDirection.Left;
				break;

			case 4:
				flagY = AimDirection.None;
				flagX = AimDirection.Left;
				break;
		}

		_aim.SetAimDirection(flagX | flagY);
	}
}
