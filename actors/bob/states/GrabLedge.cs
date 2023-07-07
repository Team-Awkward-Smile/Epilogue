using Epilogue.constants;
using Epilogue.nodes;
using Godot;

namespace Epilogue.actors.hestmor.states;
public partial class GrabLedge : StateComponent
{
	public override void OnInput(InputEvent @event)
	{
		if(Input.IsActionJustPressed(_jumpInput))
		{
			AnimPlayer.Play("ledge_climb");
			AnimPlayer.AnimationFinished += MoveToTop;
		}
		else if(Input.IsActionJustPressed(_crouchInput))
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
		Actor.GlobalPosition = new Vector2(Actor.Sprite.GlobalPosition.X, Actor.Sprite.GlobalPosition.Y + 30f);
		StateMachine.ChangeState("Idle");
	}

	public override void OnLeave()
	{
		Actor.RayCasts["Head"].Enabled = false;
		GetTree().CreateTimer(0.5f).Timeout += () => Actor.RayCasts["Head"].Enabled = true;
	}
}
