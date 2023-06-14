using Epilogue.global.enums;
using Godot;

namespace Epilogue.actors.hestmor;
public partial class Movement : Node
{
	private float _moveSpeed = 100f;
	private float _jumpSpeed = -400f;
	private float _gravity;
	private float _movementDirection;
	private bool _isJumping = false;

	private AnimationNodeStateMachinePlayback _stateMachine;
	private Sprite2D _sprite;
	private AnimationTree _animationTree;

	//public override void _Ready()
	//{
	//	_gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();
	//	_animationTree = GetNode<AnimationTree>("%AnimationTree");
	//	_stateMachine = (AnimationNodeStateMachinePlayback) _animationTree.Get("parameters/playback");
	//	_sprite = GetNode<Sprite2D>("../Sprite2D");
	//}

	//public override void _Input(InputEvent @event)
	//{
	//	if(Input.IsActionJustPressed("jump") && _parent.IsOnFloor() && _parent.TrySetAction(ActionName.Jumping))
	//	{
	//		_stateMachine.Start("Jumping_ascend");
	//	}

	//	if(Input.IsActionJustPressed("crouch") && _parent.IsOnFloor() && _parent.TrySetAction(ActionName.Crouched))
	//	{
	//		_stateMachine.Travel("Crouching");
	//	}
	//	else if(Input.IsActionJustReleased("crouch") && _parent.IsOnFloor())
	//	{
	//		_animationTree.Set("parameters/conditions/Standing", true);
	//		_stateMachine.Travel("Standing");
	//		_parent.ClearAction(ActionName.Crouched);
	//	}

	//	_movementDirection = Input.GetAxis("move_left", "move_right");

	//	if(_movementDirection == 0)
	//	{
	//		_parent.ClearAction(ActionName.Walking);
	//	}
	//	else
	//	{
	//		_sprite.Scale = new Vector2(_movementDirection < 0 ? -1 : 1, _sprite.Scale.Y);
	//	}
	//}

	//public override void _PhysicsProcess(double delta)
	//{
	//	ProcessMovement(delta);

	//	if(_isJumping && _parent.IsOnFloor())
	//	{
	//		// Lading from a jump
	//		_isJumping = false;
	//		_parent.ClearAction(ActionName.Jumping);
	//	}
	//	else if(!_isJumping && !_parent.IsOnFloor())
	//	{
	//		// Falling from a ledge
	//		_isJumping = true;
	//		_parent.TrySetAction(ActionName.Jumping);
	//		_stateMachine.Travel("Jumping_descend");
	//	}
	//}

	//public void AddJumpForce()
	//{
	//	_parent.Velocity = new Vector2(_parent.Velocity.X, _jumpSpeed);
	//	_isJumping = true;
	//}

	//private void ProcessMovement(double delta)
	//{
	//	var velocity = _parent.Velocity;

	//	velocity.Y += _gravity * (float) delta;
	//	velocity.X = _movementDirection * _moveSpeed * (_parent.CurrentAction.BlocksMovement ? 0 : 1);

	//	_parent.Velocity = velocity;

	//	_parent.MoveAndSlide();
	//}
}
