using Epilogue.actors.hestmor.enums;
using Epilogue.nodes;
using Godot;

namespace Epilogue.actors.hestmor.states;
/// <summary>
///		State that allows Hestmor to grab, hang from, and climb ledges
/// </summary>
public partial class GrabLedge : PlayerState
{
	internal override void OnInput(InputEvent @event)
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

	internal override void OnEnter(params object[] args)
	{
		Player.Velocity = new Vector2(0f, 0f);
		Player.CanChangeFacingDirection = false;
		Player.RayCasts["Feet"].ForceRaycastUpdate();

		var preffix = ((StateType) args[0] == StateType.JumpGrab) ? "jump" : "fall";

		if(Player.RayCasts["Feet"].IsColliding())
		{
			AnimPlayer.Play($"{preffix}_grab_wall");
			AnimPlayer.AnimationFinished += StayOnEdge;
		}
		else
		{
			AnimPlayer.Play($"{preffix}_grab_ledge");
		}
	}

	private void StayOnEdge(StringName animName)
	{
		AnimPlayer.AnimationFinished -= StayOnEdge;
		AnimPlayer.Play("ledge_look");
	}

	private void MoveToTop(StringName animName)
	{
		AnimPlayer.AnimationFinished -= MoveToTop;
		Player.GlobalPosition = Player.Sprite.GetNode<Node2D>("LedgeAnchor").GlobalPosition;

		StateMachine.ChangeState("Idle");
	}

	internal override void OnLeave()
	{
		Player.RayCasts["Head"].Enabled = false;
		GetTree().CreateTimer(0.5f).Timeout += () => Player.RayCasts["Head"].Enabled = true;
	}
}
