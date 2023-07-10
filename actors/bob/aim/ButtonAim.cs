using Epilogue.global.enums;
using Epilogue.nodes;
using Godot;

namespace Epilogue.actors.hestmor.aim;
public partial class ButtonAim : Node
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
		var aimDirection = Input.GetVector("aim_left_retro", "aim_right_retro", "aim_up_retro", "aim_down_retro");

		var flagX = aimDirection.X < 0 ? AimDirectionEnum.Left : aimDirection.X > 0 ? AimDirectionEnum.Right : AimDirectionEnum.None;
		var flagY = aimDirection.Y < 0 ? AimDirectionEnum.Up : aimDirection.Y > 0 ? AimDirectionEnum.Down : AimDirectionEnum.None;

		_aim.SetAimDirection(flagX | flagY);
	}
}
