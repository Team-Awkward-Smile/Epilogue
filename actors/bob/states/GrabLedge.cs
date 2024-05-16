using System.Linq;
using System.Threading.Tasks;
using Epilogue.Actors.Hestmor.Enums;
using Epilogue.Actors.VafaKeleth;
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

	internal override void OnStateMachineActivation()
	{
		AnimPlayer.AnimationFinished += (StringName animationName) =>
		{
			if (!Active || animationName != "ledge_climb")
			{
				return;
			}

			_player.GlobalPosition = _player.Sprite.GetNode<Node2D>("LedgeAnchor").GlobalPosition;

			StateMachine.ChangeState(typeof(Idle));
		};

		AnimPlayer.AnimationFinished += (StringName animationName) =>
		{
			if (!Active || animationName != "grab_wall")
			{
				return;
			}

			AnimPlayer.Play("ledge_look");
		};
	}

	internal override void OnInput(InputEvent @event)
	{
		if (@event.IsActionPressed("jump"))
		{
			AnimPlayer.Play("ledge_climb");
		}
		else if (@event.IsActionPressed("crouch"))
		{
			StateMachine.ChangeState(typeof(Fall), StateType.StandingJump);
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
		}
		else
		{
			AnimPlayer.Play("grab_ledge");
		}
	}

	internal override Task OnLeave()
	{
		_player.RayCasts["Head"].Enabled = false;

		StateMachine.GetTree().CreateTimer(0.5f).Timeout += () => _player.RayCasts["Head"].Enabled = true;

		return Task.CompletedTask;
	}
}
