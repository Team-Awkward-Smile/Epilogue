using Epilogue.Global.Enums;
using Epilogue.Nodes;
using Godot;

namespace Epilogue.Actors.Hestmor.aim;
/// <summary>
///		Node responsible for handling aiming inputs from buttons (either keyboard keys or a controller's D-Pad
/// </summary>
public partial class ButtonAim : Node
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
		var aimDirection = Input.GetVector("aim_left_retro", "aim_right_retro", "aim_up_retro", "aim_down_retro");

		var flagX = aimDirection.X < 0 ? AimDirection.Left : aimDirection.X > 0 ? AimDirection.Right : AimDirection.None;
		var flagY = aimDirection.Y < 0 ? AimDirection.Up : aimDirection.Y > 0 ? AimDirection.Down : AimDirection.None;

		_aim.SetAimDirection(flagX | flagY);
	}
}
