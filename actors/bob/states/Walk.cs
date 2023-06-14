using Epilogue.nodes;
using Godot;
using System.Linq;

namespace Epilogue.actors.hestmor.states;
public partial class Walk : StateComponent
{
	[Export] private float _moveSpeed = 100f;

	private Sprite2D _sprite;

	public override void OnInput(InputEvent @event)
	{
		if(Input.IsActionJustPressed("jump"))
		{
			StateMachine.ChangeState("Jump");
		}
	}

	public override void OnEnter()
	{
		_sprite = Character.GetChildren().OfType<Sprite2D>().FirstOrDefault();

		AnimPlayer.Play("Bob/Walking");
	}

	public override void PhysicsUpdate(double delta)
	{
		var _movementDirection = Input.GetAxis("move_left", "move_right");

		if(_movementDirection == 0f)
		{
			StateMachine.ChangeState("Idle");
			return;
		}

		if(!Character.IsOnFloor())
		{
			StateMachine.ChangeState("Fall");
			return;
		}

		_sprite.FlipH = _movementDirection < 0f;

		var velocity = Character.Velocity;

		velocity.Y += Gravity * (float) delta;
		velocity.X = _movementDirection * _moveSpeed;

		Character.Velocity = velocity;
		Character.MoveAndSlide();
	}
}
