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
		var aimDirection = Input.GetVector("aim_left_modifier", "aim_right_modifier", "aim_up_retro", "aim_down_retro");
		var flagY = AimDirectionEnum.None;
		var flagX = AimDirectionEnum.None;

		if(aimDirection.Y == 0)
		{
			// No aiming button pressed, aim Left/Right depending on character's facing position
			var actorDirection = _actor.FacingDirection;

			flagX = actorDirection == ActorFacingDirectionEnum.Left ? AimDirectionEnum.Left : AimDirectionEnum.Right;
			flagY = AimDirectionEnum.None;
		}
		else
		{
			flagY = aimDirection.Y < 0 ? AimDirectionEnum.Up : AimDirectionEnum.Down;

			if(aimDirection.X != 0)
			{
				flagX = aimDirection.X < 0 ? AimDirectionEnum.Left : AimDirectionEnum.Right;
			}
		}

		_aim.SetAimDirection(flagX | flagY);
	}
}
