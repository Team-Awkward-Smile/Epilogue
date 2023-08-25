using Epilogue.nodes;
using Godot;

namespace Epilogue.actors.hestmor.states;
public partial class GrabLedge : PlayerState
{
	public override void OnInput(InputEvent @event)
	{
		if(Input.IsActionJustPressed("jump"))
		{
			AnimPlayer.Play("ledge_climb");
			AnimPlayer.AnimationFinished += MoveToTop;
		}
		else if(Input.IsActionJustPressed("crouch"))
		{
			StateMachine.ChangeState("Fall");
		}
	}

	public override void OnEnter()
	{
		Actor.Velocity = new Vector2(0f, 0f);
		Actor.CanChangeFacingDirection = false;

		AnimPlayer.Play("grab_ledge");
		AnimPlayer.AnimationFinished += StayOnEdge;
	}

	private void StayOnEdge(StringName animName)
	{
		AnimPlayer.AnimationFinished -= StayOnEdge;
		AnimPlayer.Play("ledge_look");
	}

	private void MoveToTop(StringName animName)
	{
		AnimPlayer.AnimationFinished -= MoveToTop;
		Actor.GlobalPosition = Actor.Sprite.GetNode<Node2D>("LedgeAnchor").GlobalPosition;

		StateMachine.ChangeState("Idle");
	}

	public override void OnLeave()
	{
		Actor.RayCasts["Head"].Enabled = false;
		GetTree().CreateTimer(0.5f).Timeout += () => Actor.RayCasts["Head"].Enabled = true;
	}
}
