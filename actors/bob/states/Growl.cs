using Godot;
using Epilogue.nodes;
using Epilogue.global.enums;

namespace Epilogue.actors.hestmor.states;
/// <inheritdoc/>
public partial class Growl : State
{
	private readonly float _weakGrowlRadius;
	private readonly float _mediumGrowlRadius;
	private readonly float _strongGrowlRadius;
	private readonly Player _player;

	private Area2D _growlArea2D;
	private GrowlData _growlData;

	/// <summary>
	/// 	State that allows Hestmor to growl and taunt nearby enemies
	/// </summary>
	/// <param name="stateMachine">State that allows Hestmor to grab, hang from, and climb ledges</param>
	/// <param name="weakGrowlRadius">The radius (in pixels) of the Weak Growl</param>
	/// <param name="mediumGrowlRadius">The radius (in pixels) of the Medium Growl</param>
	/// <param name="strongGrowlRadius">The radius (in pixels) of the Strong Growl</param>
	public Growl(
		StateMachine stateMachine,
		float weakGrowlRadius,
		float mediumGrowlRadius,
		float strongGrowlRadius) : base(stateMachine)
		{
			_player = (Player) stateMachine.Owner;
			_weakGrowlRadius = weakGrowlRadius;
			_mediumGrowlRadius = mediumGrowlRadius;
			_strongGrowlRadius = strongGrowlRadius;
		}

	internal override void OnEnter(params object[] args)
	{
		_player.CanChangeFacingDirection = false;

		_growlData = _player.CurrentHealth switch
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

	/// <summary>
	/// 	Spawns an Area2D centered on Hestmor to check if any enemy was hit by the Growl
	/// </summary>
	public void SpawnArea2D()
	{
		_growlArea2D = new Area2D()
		{
			CollisionMask = (int) CollisionLayerName.Enemies
		};

		_growlArea2D.AddChild(new CollisionShape2D()
		{
			Shape = new CircleShape2D()
			{
				Radius = _growlData.AreaRadius
			}
		});

		_player.AddChild(_growlArea2D);

		_growlArea2D.BodyEntered += (Node2D body) =>
		{
			if(body is Npc enemy)
			{
				var offset = _player.SpriteSize.Y / 2;

				var raycast = new RayCast2D()
				{
					Position = new Vector2(0f, -offset),
					TargetPosition = enemy.GlobalPosition - _player.GlobalPosition,
					CollisionMask = (int) CollisionLayerName.World
				};

				_player.AddChild(raycast);

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
		StateMachine.ChangeState(typeof(Idle));
	}
}
