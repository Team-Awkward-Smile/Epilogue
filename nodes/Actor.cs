using Epilogue.global.enums;
using Godot;
using System.Collections.Generic;
using System.Linq;

namespace Epilogue.nodes;
[GlobalClass, Tool]
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

	public override string[] _GetConfigurationWarnings()
	{
		var warnings = new List<string>();

		foreach(var c in GetChildren())
		{
			GD.Print(c.Name + " " + (c is Health));
		}


		if(!GetChildren().Where(c => c.Name == "FlipRoot").Any())
		{
			warnings.Add("This Actor has no Flip Root.\nConsider adding a Node2D called FlipRoot to manage children Nodes that need to rotate during the game");
		}

		if(!GetChildren().OfType<Health>().Any())
		{
			warnings.Add("This Actor has no Health.\nConsider adding a Health Node as a child of this Node to control the Actor's HP during the game");
		}

		if(!GetChildren().OfType<StateMachineComponent>().Any())
		{
			warnings.Add("This Actor has no State Machine.\nConsider adding a StateMachine Node as a child of this Actor to control the Actor's States during the game");
		}

		if(!GetChildren().OfType<AudioPlayerBase>().Any())
		{
			warnings.Add("This Actor has no ActorAudioPlayer.\nConsider adding an ActorAudioPlayer Node as a child of this Actor to allow this Actor to play sound effects during the game");
		}

		return warnings.ToArray();
	}

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
