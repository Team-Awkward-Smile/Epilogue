using Godot;
using System.Linq;
using System.Threading.Tasks;

namespace Epilogue.nodes;
/// <summary>
///		Node representing a State that an Actor can assume when using a State Machine
/// </summary>
[GlobalClass, Icon("res://nodes/state.png")]
public partial class State : Node
{
	protected StateMachine StateMachine { get; private set; }
	protected Actor Actor { get; private set; }
	protected AnimationPlayer AnimPlayer { get; private set; }
	protected Area2D HitBoxContainer { get; private set; }
	protected ActorAudioPlayer AudioPlayer { get; private set; }
	protected float Gravity { get; private set; }

	public override void _Ready()
	{
		StateMachine = (StateMachine) GetParent();

		if(StateMachine is null)
		{
			GD.PrintErr($"PlayerState Machine not found for PlayerState [{Name}]");
		}

		Actor = (Actor) StateMachine.GetParent();
		AnimPlayer = Actor.GetChildren().OfType<AnimationPlayer>().FirstOrDefault();

		if(AnimPlayer is null)
		{
			GD.PrintErr($"Animation Player not found for Actor [{Actor.Name}]");
		}

		HitBoxContainer = Actor.GetNode<Area2D>("FlipRoot/HitBoxContainer");

		if(HitBoxContainer is null)
		{
			GD.PrintErr($"Hitbox Container not found for Actor [{Actor.Name}]");
		}

		Gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();

		AudioPlayer = Actor.GetChildren().OfType<ActorAudioPlayer>().FirstOrDefault();

		if(AudioPlayer is null)
		{
			GD.PrintErr($"Audio Player not found for Actor [{Actor.Name}]");
		}
	}

	public virtual void Update(double delta) { }

	public virtual void PhysicsUpdate(double delta) { }

	public virtual void OnEnter() { }

	public virtual void OnLeave() { }

	public virtual async Task OnLeaveAsync() => await Task.CompletedTask;

	public virtual void OnInput(InputEvent @event) { }
}
