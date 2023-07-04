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
		var flagY = AimDirection.None;
		var flagX = AimDirection.None;

		if(aimDirection.Y == 0)
		{
			// No aiming button pressed, aim Left/Right depending on character's facing position
			var actorDirection = _actor.FacingDirection;

			flagX = actorDirection == ActorFacingDirection.Left ? AimDirection.Left : AimDirection.Right;
			flagY = AimDirection.None;
		}
		else
		{
			flagY = aimDirection.Y < 0 ? AimDirection.Up : AimDirection.Down;

			if(aimDirection.X != 0)
			{
				flagX = aimDirection.X < 0 ? AimDirection.Left : AimDirection.Right;
			}
		}

		_aim.SetAimDirection(flagX | flagY);
	}
}
