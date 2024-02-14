using System.Linq;
using System.Threading.Tasks;
using Epilogue.Actors.Hestmor.Enums;
using Epilogue.Nodes;
using Godot;

namespace Epilogue.Actors.Hestmor.States;
/// <inheritdoc/>
public partial class GrabLedge : State
{
	private readonly Player _player;

	/// <summary>
	/// 	State that allows Hestmor to grab, hang from, and climb ledges
	/// </summary>
	/// <param name="stateMachine">State that allows Hestmor to fall from high places</param>
	public GrabLedge(StateMachine stateMachine) : base(stateMachine)
	{
		_player = (Player)stateMachine.Owner;
	}

	internal override void OnInput(InputEvent @event)
	{
		if (Input.IsActionJustPressed("jump"))
		{
			AnimPlayer.Play("ledge_climb");
			AnimPlayer.AnimationFinished += MoveToTop;
		}
		else if (Input.IsActionJustPressed("crouch"))
		{
			StateMachine.ChangeState(typeof(Fall), StateType.StandingJump);

			var c = AnimPlayer.GetSignalConnectionList(AnimationMixer.SignalName.AnimationFinished).FirstOrDefault()?["callable"];

			if (c is not null)
			{
				AnimPlayer.Disconnect(AnimationMixer.SignalName.AnimationFinished, (Callable)c);
			}
		}
	}

	internal override void OnEnter(params object[] args)
	{
		_player.Velocity = new Vector2(0f, 0f);
		_player.CanChangeFacingDirection = false;
		_player.RayCasts["Feet"].ForceRaycastUpdate();

		if (_player.RayCasts["Feet"].IsColliding())
		{
			AnimPlayer.Play("grab_wall");
			AnimPlayer.AnimationFinished += StayOnEdge;
		}
		else
		{
			AnimPlayer.Play("grab_ledge");
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
		_player.GlobalPosition = _player.Sprite.GetNode<Node2D>("LedgeAnchor").GlobalPosition;

		StateMachine.ChangeState(typeof(Idle));
	}

	internal override Task OnLeave()
	{
		_player.RayCasts["Head"].Enabled = false;

		StateMachine.GetTree().CreateTimer(0.5f).Timeout += () => _player.RayCasts["Head"].Enabled = true;

		return Task.CompletedTask;
	}
}
