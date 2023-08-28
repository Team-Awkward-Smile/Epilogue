using Epilogue.global.enums;
using Epilogue.nodes;
using Godot;
using System;

namespace Epilogue.actors.hestmor.aim;
/// <summary>
///		Node responsible for handling aiming inputs from a joystick
/// </summary>
public partial class StickAim : Node
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
		var aimVector = Input.GetVector("aim_left_controller_modern", "aim_right_controller_modern", "aim_down_controller_modern", "aim_up_controller_modern");
		var flagX = AimDirection.None;
		var flagY = AimDirection.None;

		if(aimVector.X <= -0.5f)
		{
			flagX = AimDirection.Left;
		}
		else if(aimVector.X >= 0.5f)
		{
			flagX = AimDirection.Right;
		}

		if(aimVector.Y <= -0.5f)
		{
			flagY = AimDirection.Down;
		}
		else if(aimVector.Y >= 0.5)
		{
			flagY = AimDirection.Up;
		}

		if((flagX | flagY) == AimDirection.None)
		{
			flagX = _actor.FacingDirection == ActorFacingDirection.Left ? AimDirection.Left : AimDirection.Right;
		}

		_aim.SetAimDirection(flagX | flagY);
	}
}
