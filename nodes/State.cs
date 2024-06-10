using Godot;
using System.Linq;
using System.Threading.Tasks;

namespace Epilogue.Nodes;
/// <summary>
///		Node representing a State that an Actor can assume when using a State Machine
/// </summary>
public class State
{
	/// <summary>
	///		State Machine responsible for controlling this and other States
	/// </summary>
	protected StateMachine StateMachine { get; private set; }

	/// <summary>
	///		AnimationPlayer of the Actor. Used by States to controls animations
	/// </summary>
	protected AnimationPlayer AnimPlayer { get; private set; }

	/// <summary>
	///		Audio Player of the Actor. Used by States to play sound effects
	/// </summary>
	protected ActorAudioPlayer AudioPlayer { get; private set; }

	/// <inheritdoc/>
	public State(StateMachine stateMachine)
	{
		StateMachine = stateMachine;
		AnimPlayer = StateMachine.Owner.GetChildren().OfType<AnimationPlayer>().FirstOrDefault();

		if (AnimPlayer is null)
		{
			GD.PrintErr($"Animation Player not found for Actor [{StateMachine.Owner.Name}]");
		}

		AudioPlayer = StateMachine.Owner.GetNode<ActorAudioPlayer>("FlipRoot/ActorAudioPlayer");

		if (AudioPlayer is null)
		{
			GD.PrintErr($"Audio Player not found for Actor [{StateMachine.Owner.Name}]");
		}
	}

	/// <summary>
	///		Method executed at every frame
	/// </summary>
	/// <param name="delta">Time in seconds since the last frame</param>
	internal virtual void Update(double delta) { }

	/// <summary>
	///		Method executed at every physical frame (60 times per seconds, by default, regardless of the current framerate)
	/// </summary>
	/// <param name="delta">Time in seconds since the last physical frame (this value ideally is static)</param>
	internal virtual void PhysicsUpdate(double delta) { }

	/// <summary>
	///		Method executed every time a State becomes active
	/// </summary>
	internal virtual void OnEnter(params object[] args) { }

	/// <summary>
	///		Method executed every time a State is replaced by another one, right before the exchange occurs
	/// </summary>
	internal virtual Task OnLeave() { return Task.CompletedTask; }

	/// <summary>
	///		Method executed every time an Unhandled Input is detected
	/// </summary>
	/// <param name="event">The input event received</param>
	internal virtual void OnInput(InputEvent @event) { }
}
