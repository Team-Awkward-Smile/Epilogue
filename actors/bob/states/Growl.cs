using Godot;
using Epilogue.nodes;
using Epilogue.global.enums;
using Epilogue.actors.hestmor;

namespace Epilogue.actors.hestmot.states;
/// <summary>
///		State that allows Hestmor to growl and taunt nearby enemies
/// </summary>
public partial class Growl : PlayerState
{
	[Export] private float _weakGrowlRadius;
	[Export] private float _mediumGrowlRadius;
	[Export] private float _strongGrowlRadius;

	private Area2D _growlArea2D;
	private GrowlData _growlData;

	internal override void OnEnter(params object[] args)
	{
		Player.CanChangeFacingDirection = false;

		_growlData = Player.CurrentHealth switch
		{
			1 => new GrowlData()
			{
				Type = GrowlType.Strong,
				Animation = "growl_strong",
				Strength = 300f,
				StrengthFallOff = 5,
				AreaRadius = _strongGrowlRadius,
				MinimumDistance = 200f
			},
			2 => new GrowlData()
			{
				Type = GrowlType.Medium,
				Animation = "growl_medium",
				Strength = 200f,
				StrengthFallOff = 4,
				AreaRadius = _mediumGrowlRadius,
				MinimumDistance = 100f,
			},
			_ => new GrowlData()
			{
				Type = GrowlType.Weak,
				Animation = "growl_weak",
				Strength = 20f,
				StrengthFallOff = 2,
				AreaRadius = _weakGrowlRadius,
				MinimumDistance = 50f
			}
		};

		AnimPlayer.Play($"Growl/{_growlData.Animation}");
		AnimPlayer.AnimationFinished += EndGrowl;
	}

	public void SpawnArea2D()
	{
		_growlArea2D = new Area2D()
		{
			CollisionMask = (int) (CollisionLayerName.Enemies)
		};

		_growlArea2D.AddChild(new CollisionShape2D()
		{
			Shape = new CircleShape2D()
			{
				Radius = _growlData.AreaRadius
			}
		});

		Player.AddChild(_growlArea2D);

		_growlArea2D.BodyEntered += (Node2D body) =>
		{
			if(body is Npc enemy)
			{
				var offset = Player.SpriteSize.Y / 2;

				var raycast = new RayCast2D()
				{
					Position = new Vector2(0f, -offset),
					TargetPosition = enemy.GlobalPosition - Player.GlobalPosition,
					CollisionMask = (int) CollisionLayerName.World
				};

				Player.AddChild(raycast);

				raycast.ForceRaycastUpdate();

				var distance = raycast.GlobalPosition.DistanceTo(enemy.GlobalPosition);
				var penalty = raycast.IsColliding() ? 200f : 0f;

				if(distance < _growlData.MinimumDistance)
				{
					distance = 0f;
				}

				enemy.ReactToGrowl(_growlData.Strength - (distance * 0.2f / _growlData.StrengthFallOff) - penalty);
				raycast.QueueFree();
			}
		};
	}

	private void EndGrowl(StringName animName)
	{
		_growlArea2D?.QueueFree();
		AnimPlayer.AnimationFinished -= EndGrowl;
		StateMachine.ChangeState("Idle");
	}
}
