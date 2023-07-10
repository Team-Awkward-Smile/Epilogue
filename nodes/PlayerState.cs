using Epilogue.util;

namespace Epilogue.nodes;
/// <summary>
///		Node representing a State that a Player Character can assume when using a State Machine.
///		Includes strings mapped to actions the player can perform
/// </summary>
public partial class PlayerState : State
{
	protected readonly string _jumpInput = InputUtils.GetInputActionName("jump");
	protected readonly string _attackInput = InputUtils.GetInputActionName("melee");
	protected readonly string _crouchInput = InputUtils.GetInputActionName("crouch");
	protected readonly string _moveLeftInput = InputUtils.GetInputActionName("move_left");
	protected readonly string _moveRightInput = InputUtils.GetInputActionName("move_right");
	protected readonly string _toggleRunInput = InputUtils.GetInputActionName("toggle_run");
	protected readonly string _lookUpInput = InputUtils.GetInputActionName("look_up");
	protected readonly string _slideInput = InputUtils.GetInputActionName("slide");
	protected readonly string _cancelSlideInput = InputUtils.GetInputActionName("cancel_slide");
	protected readonly string _growlInput = InputUtils.GetInputActionName("growl");
}
