using Epilogue.global.enums;
using Epilogue.nodes;
using Godot;

namespace Epilogue.actors.hestmor.aim;
public partial class MouseAim : Node
{
	private Actor _actor;
	private Aim _aim;

	public override void _Ready()
	{
		_actor = (Actor) Owner;
		_aim = (Aim) GetParent();
	}

	public override void _Process(double delta)
	{
		var mousePosition = GetViewport().GetMousePosition();
		var screenSize = DisplayServer.WindowGetSize();
		var flagX = AimDirectionEnum.None;
		var flagY = AimDirectionEnum.None;

		switch((int) mousePosition.X / (screenSize.X / 3))
		{
			case 0:
				flagX = AimDirectionEnum.Left;
				break;

			case 2:
				flagX = AimDirectionEnum.Right;
				break;
		}

		switch((int) mousePosition.Y / (screenSize.Y / 3))
		{
			case 0:
				flagY = AimDirectionEnum.Up;
				break;

			case 2:
				flagY = AimDirectionEnum.Down;
				break;
		}

		if((flagX | flagY) == AimDirectionEnum.None)
		{
			flagX = _actor.FacingDirection == ActorFacingDirectionEnum.Left ? AimDirectionEnum.Left : AimDirectionEnum.Right;
		}

		_aim.SetAimDirection(flagX | flagY);
	}
}
