using Epilogue.global.enums;
using Epilogue.nodes;
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
		var mousePosition = GetViewport().GetMousePosition();
		var screenSize = DisplayServer.WindowGetSize();
		var flagX = AimDirection.None;
		var flagY = AimDirection.None;

		switch((int) mousePosition.X / (screenSize.X / 3))
		{
			case 0:
				flagX = AimDirection.Left;
				break;

			case 2:
				flagX = AimDirection.Right;
				break;
		}

		switch((int) mousePosition.Y / (screenSize.Y / 3))
		{
			case 0:
				flagY = AimDirection.Up;
				break;

			case 2:
				flagY = AimDirection.Down;
				break;
		}

		if((flagX | flagY) == AimDirection.None)
		{
			flagX = _actor.FacingDirection == ActorFacingDirection.Left ? AimDirection.Left : AimDirection.Right;
		}

		_aim.SetAimDirection(flagX | flagY);
	}
}
