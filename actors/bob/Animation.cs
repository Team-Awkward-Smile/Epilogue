using Godot;

namespace Actors.Hestmor;
public partial class Animation : AnimatedSprite2D
{
	public override void _Input(InputEvent @event)
	{
		var movement = Input.GetAxis("move_left", "move_right");

		if(movement == 0f)
		{
			Play("idle");
		}
		else
		{
			Play("walk");

			FlipH = movement < 0f;
		}
	}
}
