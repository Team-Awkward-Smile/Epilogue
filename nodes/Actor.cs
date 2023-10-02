using Epilogue.global.enums;
using Godot;
using Godot.Collections;
using System.Linq;

namespace Epilogue.nodes;
/// <summary>
///		Base class for all Actors in the game. Instead of using this one, you probably should use either <see cref="Player"/> or <see cref="Npc"/>
/// </summary>
[GlobalClass]
public abstract partial class Actor : CharacterBody2D
{
	/// <summary>
	///		Max HP of this Actor
	/// </summary>
    [Export] public float MaxHealth { get; private protected set; }

	/// <summary>
	///		Current HP of this Actor
	/// </summary>
    [Export] public float CurrentHealth { get; private protected set; }

	/// <summary>
	///		All RayCast2D's belonging to this Actor, accessed by their names (minus the 'RayCast2D' suffix)
	/// </summary>
	public Dictionary<string, RayCast2D> RayCasts { get; set; } = new();

	/// <summary>
	///		Direction (Left/Right) this Actor is currently facing
	/// </summary>
    public ActorFacingDirection FacingDirection { get; private set; } = ActorFacingDirection.Right;

	/// <summary>
	///		Defines if this Actor can change the direction it is facing
	/// </summary>
	public bool CanChangeFacingDirection { get; set; } = true;

	/// <summary>
	///		Reference to the main Sprite used by the Actor
	/// </summary>
    public Sprite2D Sprite { get; set; }

	/// <summary>
	///		Reference to the State Machine used by the Actor
	/// </summary>
    public StateMachine StateMachine { get; set; }

	private protected AnimationPlayer AnimationPlayer { get; set; }

	private protected HurtBox HurtBox { get; set; }

    /// <inheritdoc/>
    public override void _Ready()
	{
		GetNode<Node2D>("FlipRoot").GetChildren().OfType<RayCast2D>().ToList().ForEach(r =>
		{
			RayCasts.Add(r.Name.ToString().Replace("RayCast2D", ""), r);
		});

		Sprite = GetNode<Node2D>("FlipRoot").GetChildren().OfType<Sprite2D>().Where(c => c.IsInGroup("MainSprite")).FirstOrDefault();
		StateMachine = GetChildren().OfType<StateMachine>().FirstOrDefault();
		AnimationPlayer = GetChildren().OfType<AnimationPlayer>().FirstOrDefault();
		HurtBox = GetChildren().OfType<HurtBox>().FirstOrDefault();

		HurtBox.AreaEntered += (Area2D area) =>
		{
			if(area is HitBox hitbox)
			{
				DealDamage(hitbox.Damage);
			}
		};
		
		AfterReady();
	}

	/// <summary>
	///		Method to allow an Actor to initialize custom logic in <see cref="_Ready"/> without overriding any of the existing code
	/// </summary>
	private protected virtual void AfterReady() { }

	/// <summary>
	///		Sets a new facing direction for this Actor, if the informed direction is valid and <see cref="CanChangeFacingDirection"/> is true
	/// </summary>
	/// <param name="newDirection">The flag enum representing the new direction</param>
	public void SetFacingDirection(ActorFacingDirection newDirection)
	{
		if(!CanChangeFacingDirection)
		{
			return;
		}

		var flipNode = GetNode<Node2D>("FlipRoot");
		var scaleX = newDirection switch
		{
			ActorFacingDirection.Left => -1,
			ActorFacingDirection.Right => 1,
			_ => 1
		};

		FacingDirection = newDirection;

		flipNode.Scale = new Vector2(scaleX, 1f);
	}

	/// <summary>
	///		Just like the regular MoveAndSlide, but rotates the Actor if the movement occurred on a slope
	/// </summary>
	public void MoveAndSlideWithRotation()
	{
		MoveAndSlide();

		if(IsOnFloor())
		{
			var floorRadianAngle = GetFloorAngle();
			var floorNormal = GetFloorNormal();

			if(floorRadianAngle is > 0 and < 1)
			{
				CreateTween().TweenProperty(this, "rotation", floorRadianAngle * (floorNormal.X > 0 ? 1 : -1), 0.05f);
			}
			else if(floorRadianAngle == 0)
			{
				CreateTween().TweenProperty(this, "rotation", 0f, 0.05f);
			}
		}
		else
		{
			CreateTween().TweenProperty(this, "rotation", 0f, 0.05f);
		}
	}

	/// <summary>
	///		Deals the indicated ammount of damage to this Actor
	/// </summary>
	/// <param name="damage">The ammount of damage to deal to this Actor</param>
	public abstract void DealDamage(float damage);

	/// <summary>
	///		Heals the indicated ammount of HP to this Actor.
	/// </summary>
	/// <param name="health">The ammount of HP to recover</param>
	public abstract void ApplyHealth(float health);
}
