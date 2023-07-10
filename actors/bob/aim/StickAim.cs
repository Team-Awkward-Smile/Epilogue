using Epilogue.global.enums;
using Epilogue.nodes;
using Godot;
using System;

namespace Epilogue.actors.hestmor.aim;
public partial class StickAim : Node
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
		var aimVector = Input.GetVector("aim_left_controller_modern", "aim_right_controller_modern", "aim_down_controller_modern", "aim_up_controller_modern");
		var flagX = AimDirectionEnum.None;
		var flagY = AimDirectionEnum.None;

		if(aimVector.X <= -0.5f)
		{
			flagX = AimDirectionEnum.Left;
		}
		else if(aimVector.X >= 0.5f)
		{
			flagX = AimDirectionEnum.Right;
		}

		if(aimVector.Y <= -0.5f)
		{
			flagY = AimDirectionEnum.Down;
		}
		else if(aimVector.Y >= 0.5)
		{
			flagY = AimDirectionEnum.Up;
		}

		if((flagX | flagY) == AimDirectionEnum.None)
		{
			flagX = _actor.FacingDirection == ActorFacingDirectionEnum.Left ? AimDirectionEnum.Left : AimDirectionEnum.Right;
		}

		_aim.SetAimDirection(flagX | flagY);
	}
}
