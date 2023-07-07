using Epilogue.global.enums;
using Godot;
using Godot.Collections;
using System.Linq;

namespace Epilogue.nodes;
[GlobalClass]
public partial class Actor : CharacterBody2D
{
	/// <summary>
	///		All RayCast2D's belonging to this Actor, accessed by their names (minus the 'RayCast2D' suffix)
	/// </summary>
	public Dictionary<string, RayCast2D> RayCasts { get; set; } = new();

	/// <summary>
	///		Direction (Left/Right) this Actor is currently facing
	/// </summary>
    public ActorFacingDirectionEnum FacingDirection { get; private set; } = ActorFacingDirectionEnum.Right;

	public bool CanChangeFacingDirection { get; set; } = true;

    public Sprite2D Sprite { get; set; }

    public Health Health { get; set; }

    public override void _Ready()
	{
		GetNode<Node2D>("FlipRoot").GetChildren().OfType<RayCast2D>().ToList().ForEach(r =>
		{
			RayCasts.Add(r.Name.ToString().Replace("RayCast2D", ""), r);
		});

		Sprite = GetNode<Node2D>("FlipRoot").GetChildren().OfType<Sprite2D>().Where(c => c.IsInGroup("MainSprite")).FirstOrDefault();
		Health = GetChildren().OfType<Health>().FirstOrDefault();
	}

	public void SetFacingDirection(ActorFacingDirectionEnum newDirection)
	{
		if(!CanChangeFacingDirection)
		{
			return;
		}

		var flipNode = GetNode<Node2D>("FlipRoot");
		var scaleX = newDirection switch
		{
			ActorFacingDirectionEnum.Left => -1,
			ActorFacingDirectionEnum.Right => 1,
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
}
