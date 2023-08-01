using Epilogue.util;
using Godot;
using System.Linq;
using System.Threading.Tasks;

namespace Epilogue.nodes;
public partial class StateComponent : Node
{
	[Signal] public delegate void StateStartedEventHandler();
	[Signal] public delegate void StateFinishedEventHandler();

	protected readonly string _jumpInput = InputUtils.GetInputActionName("jump");
	protected readonly string _attackInput = InputUtils.GetInputActionName("melee");
	protected readonly string _crouchInput = InputUtils.GetInputActionName("crouch");
	protected readonly string _moveLeftDigitalInput = InputUtils.GetInputActionName("move_left_digital");
	protected readonly string _moveRightDigitalInput = InputUtils.GetInputActionName("move_right_digital");
	protected readonly string _moveLeftAnalogInput = "move_left_analog_modern";
	protected readonly string _moveRightAnalogInput = "move_right_analog_modern";
	protected readonly string _runModifierInput = InputUtils.GetInputActionName("run_modifier");
	protected readonly string _lookUpInput = InputUtils.GetInputActionName("look_up");
	protected readonly string _slideInput = InputUtils.GetInputActionName("slide");
	protected readonly string _cancelSlideInput = InputUtils.GetInputActionName("cancel_slide");
	protected readonly string _growlInput = InputUtils.GetInputActionName("growl");

	protected StateMachineComponent StateMachine { get; private set; }
	protected Actor Actor { get; private set; }
	protected Player Player { get; private set; }
	protected AnimationPlayer AnimPlayer { get; private set; }
	protected AudioPlayerBase AudioPlayer { get; private set; }
	protected float Gravity { get; private set; }

	public override void _Ready()
	{
		StateMachine = (StateMachineComponent) GetParent();

		if(StateMachine is null)
		{
			GD.PrintErr($"State Machine not found for State [{Name}]");
		}

		Actor = (Actor) StateMachine.GetParent();

		if(Actor is Player player)
		{
			Player = player;
		}

		AnimPlayer = Actor.GetChildren().OfType<AnimationPlayer>().FirstOrDefault();

		if(AnimPlayer is null)
		{
			GD.PrintErr($"Animation Player not found for Actor [{Actor.Name}]");
		}

		Gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();
		AudioPlayer = Actor.GetChildren().OfType<AudioPlayerBase>().FirstOrDefault();

		if(AudioPlayer is null)
		{
			GD.PrintErr($"Audio Player not found for Actor [{Actor.Name}]");
		}
	}

	/// <summary>
	///		Method executed at every frame
	/// </summary>
	/// <param name="delta">Time in seconds since the last frame</param>
	public virtual void Update(double delta) { }

	/// <summary>
	///		Method executed at every physical frame (usually 60 times per second, regardless of framerate)
	/// </summary>
	/// <param name="delta">Time in secondssince last frame</param>
	/// <inheritdoc/>
	public virtual void PhysicsUpdate(double delta) { }

	/// <summary>
	///		Method executed when this State becomes active. Note that this method runs every time the State becomes active, so it's not a replacement for <see cref="_Ready"/>
	/// </summary>
	public virtual void OnEnter() => EmitSignal(SignalName.StateStarted);

	/// <summary>
	///		Synchronous method executed when a State is replaced by another one. If a State needs to run async operations when leaving the State Machine, use <see cref="OnLeaveAsync"/>
	/// </summary>
	public virtual void OnLeave() => EmitSignal(SignalName.StateFinished);

	/// <summary>
	///		Async method executed when a State is replaced by another one. The State Machine will await for this method before continuing the change
	/// </summary>
	/// <returns></returns>
	public async virtual Task OnLeaveAsync()
	{
		EmitSignal(SignalName.StateFinished);
		await Task.CompletedTask;
	}

	/// <summary>
	///		Method executed when an Unhandled Input is received
	/// </summary>
	/// <param name="event"></param>
	public virtual void OnInput(InputEvent @event) { }
}
