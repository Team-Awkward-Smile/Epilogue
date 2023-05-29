using Godot;
using System;

namespace Actors.Hestmor;
public partial class Movement : CharacterBody2D
{
	private float _moveSpeed = 100f;
	private float _jumpSpeed = -400f;
	private float _gravity;

	public override void _Ready()
	{
		_gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();
	}

	public override void _Input(InputEvent @event)
	{
		if(Input.IsActionJustPressed("jump") && IsOnFloor())
		{
			Velocity = new Vector2(Velocity.X, _jumpSpeed);
		}
	}

	public override void _PhysicsProcess(double delta)
	{
		var velocity = Velocity;
		var direction = Input.GetAxis("move_left", "move_right");

		velocity.Y += _gravity * (float) delta;
		velocity.X = direction * _moveSpeed;

		Velocity = velocity;

		MoveAndSlide();
	}
}
