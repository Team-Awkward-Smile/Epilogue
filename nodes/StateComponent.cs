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
	protected readonly string _moveLeftInput = InputUtils.GetInputActionName("move_left");
	protected readonly string _moveRightInput = InputUtils.GetInputActionName("move_right");
	protected readonly string _toggleRunInput = InputUtils.GetInputActionName("toggle_run");
	protected readonly string _lookUpInput = InputUtils.GetInputActionName("look_up");
	protected readonly string _slideInput = InputUtils.GetInputActionName("slide");
	protected readonly string _cancelSlideInput = InputUtils.GetInputActionName("cancel_slide");

	protected StateMachineComponent StateMachine { get; private set; }
	protected Actor Actor { get; private set; }
	protected AnimationPlayer AnimPlayer { get; private set; }
	protected Area2D HitBoxContainer { get; private set; }
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
		AnimPlayer = Actor.GetChildren().OfType<AnimationPlayer>().FirstOrDefault();

		if(AnimPlayer is null)
		{
			GD.PrintErr($"Animation Player not found for Actor [{Actor.Name}]");
		}

		HitBoxContainer = Actor.GetNode<Area2D>("RotationContainer/HitBoxContainer");

		if(HitBoxContainer is null)
		{
			GD.PrintErr($"Hitbox Container not found for Actor [{Actor.Name}]");
		}

		Gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();

		AudioPlayer = Actor.GetChildren().OfType<AudioPlayerBase>().FirstOrDefault();

		if(AudioPlayer is null)
		{
			GD.PrintErr($"Audio Player not found for Actor [{Actor.Name}]");
		}
	}

	public virtual void Update(double delta) { }

	public virtual void PhysicsUpdate(double delta) { }

	public virtual void OnEnter() => EmitSignal(SignalName.StateStarted);

	public virtual void OnLeave() => EmitSignal(SignalName.StateFinished);

	public async virtual Task OnLeaveAsync()
	{
		EmitSignal(SignalName.StateFinished);
		await Task.CompletedTask;
	}

	public virtual void OnInput(InputEvent @event) { }
}
