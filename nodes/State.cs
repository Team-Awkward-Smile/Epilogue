using Godot;

using System.Linq;
using System.Threading.Tasks;

namespace Epilogue.nodes;
/// <summary>
///		Node representing a State that an Actor can assume when using a State Machine
/// </summary>
[GlobalClass, Icon("res://nodes/icons/state.png")]
public partial class State : Node
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
	public override void _Ready()
	{
		StateMachine = (StateMachine) GetParent();

		if(StateMachine is null)
		{
			GD.PrintErr($"PlayerState Machine not found for PlayerState [{Name}]");
		}

		AnimPlayer = Owner.GetChildren().OfType<AnimationPlayer>().FirstOrDefault();

		if(AnimPlayer is null)
		{
			GD.PrintErr($"Animation Player not found for Actor [{Owner.Name}]");
		}

		AudioPlayer = Owner.GetChildren().OfType<ActorAudioPlayer>().FirstOrDefault();

		if(AudioPlayer is null)
		{
			GD.PrintErr($"Audio Player not found for Actor [{Owner.Name}]");
		}

		AfterReady();
	}

	/// <summary>
	///		Method executed after <see cref="_Ready"/> finishes executing. Allows a State to initialize custom logic without overriding the default implementation of _Ready
	/// </summary>
	private protected virtual void AfterReady() { }

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
	///		Method executed every time a State becomes active. Since it can run multiple times during the game, it is not a replacement for <see cref="_Ready"/>
	/// </summary>
	internal virtual void OnEnter(params object[] args) { }

	/// <summary>
	///		Method executed every time a State is replaced by another one, right before the exchange occurs. If any async operation needs to be executed here, use <see cref="OnLeaveAsync"/> instead
	/// </summary>
	internal virtual void OnLeave() { }

	/// <summary>
	///		Async method executed every time a State is replaced by another one. The State Machine responsible for the State will await for this method before finishing the exchange and setting a new State
	/// </summary>
	/// <returns></returns>
	internal virtual async Task OnLeaveAsync()
	{
		await Task.CompletedTask;
	}

	/// <summary>
	///		Method executed every time an Unhandled Input is detected
	/// </summary>
	/// <param name="event">The input event received</param>
	internal virtual void OnInput(InputEvent @event) { }
}
