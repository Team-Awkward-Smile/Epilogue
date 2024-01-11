using System.Threading.Tasks;
using Epilogue.Nodes;
using Godot;

namespace Epilogue.actors.rob.states;
/// <inheritdoc/>
public partial class Attack : State
{
	private readonly Rob _rob;

	private Callable _callable;

	/// <summary>
	/// 	State that allows Rob to perform melee attacks
	/// </summary>
	/// <param name="stateMachine">The State Machine who owns this State</param>
	public Attack(StateMachine stateMachine) : base(stateMachine)
	{
		_rob = (Rob) stateMachine.Owner;
	}

	internal override void OnEnter(params object[] args)
	{
		_rob.CanChangeFacingDirection = false;
		_callable = Callable.From((string animName) => OnAnimationFinished());

		AnimPlayer.PlayBackwards("Combat/attack");
		AnimPlayer.Connect(AnimationMixer.SignalName.AnimationFinished, _callable);
	}

	private void OnAnimationFinished()
	{
		AnimPlayer.Disconnect(AnimationMixer.SignalName.AnimationFinished, _callable);
		StateMachine.ChangeState(typeof(Move));
	}

	internal override Task OnLeave()
	{
		// Prevent cases where an attack was interrupted mid-animation and the signals where not cleared properly
		if(AnimPlayer.IsConnected(AnimationMixer.SignalName.AnimationFinished, _callable))
		{
			AnimPlayer.Disconnect(AnimationMixer.SignalName.AnimationFinished, _callable);
		}

		_rob.CanChangeFacingDirection = true;

		return Task.CompletedTask;
	}

	/// <summary>
	/// 	Spawns a HitBox to check for collisions against the player
	/// </summary>
	public void SpawnHitBox()
	{
		var area = StateMachine.GetNode<HitBox>("../../FlipRoot/HitBox");

		area.Damage = 1f;
		area.CollisionShape = GD.Load<CircleShape2D>("res://actors/rob/hitboxes/slam.tres");
	}

	/// <summary>
	/// 	Destroyes a previously created HitBox
	/// </summary>
	public void DestroyHitBox()
	{
		var area = StateMachine.GetNode<HitBox>("../../FlipRoot/HitBox");

		area.DeleteHitBox();
	}
}
