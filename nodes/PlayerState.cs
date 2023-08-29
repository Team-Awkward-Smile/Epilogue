using Epilogue.global.singletons;
using Epilogue.util;

namespace Epilogue.nodes;
/// <summary>
///		Node representing a State that a Player Character can assume when using a State Machine.
///		Includes strings mapped to actions the player can perform
/// </summary>
public partial class PlayerState : State
{
	/// <summary>
	///		Reference to the player character
	/// </summary>
	private protected Player Player { get; private set; }

	/// <summary>
	///		Singleton responsible for triggering events related to the player character
	/// </summary>
	private protected PlayerEvents PlayerEvents { get; private set; }

	private protected string JumpInput { get; } = InputUtils.GetInputActionName("jump");
	private protected string CrouchInput { get; } = InputUtils.GetInputActionName("crouch");
	private protected string MoveLeftDigitalInput { get; } = InputUtils.GetInputActionName("move_left_digital");
	private protected string MoveRightDigitalInput { get; } = InputUtils.GetInputActionName("move_right_digital");
	private protected string MoveLeftAnalogInput { get; } = "move_left_analog_modern";
	private protected string MoveRightAnalogInput { get; } = "move_right_analog_modern";
	private protected string LookUpInput { get; } = InputUtils.GetInputActionName("look_up");
	private protected string SlideInput { get; } = InputUtils.GetInputActionName("slide");
	private protected string CancelSlideInput { get; } = InputUtils.GetInputActionName("cancel_slide");
	private protected string MeleeAttackInput { get; } = InputUtils.GetInputActionName("melee");
	private protected string GrowlInput { get; } = InputUtils.GetInputActionName("growl");

	private protected override void AfterReady()
	{
		Player = (Player) Owner;
		PlayerEvents = GetNode<PlayerEvents>("/root/PlayerEvents");
	}
}
