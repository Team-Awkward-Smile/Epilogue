using Epilogue.Global.DTO;
using Epilogue.Global.Enums;
using Epilogue.Nodes;
using Godot;

namespace Epilogue.Actors.Hestmor;
/// <summary>
///		Node that manages the Growl area and it's effects
/// </summary>
public partial class GrowlEffectArea : Area2D
{
	private Player _player;
	private CollisionShape2D _collisionShape;
	private GrowlDTO _growlDto;

	/// <inheritdoc/>
	public override void _Ready()
	{
		_player = (Player)Owner;
		_collisionShape = GetChild(0) as CollisionShape2D;
	}

	/// <summary>
	///		Sets up the Growl data so they can be correctly sent to the right places
	/// </summary>
	/// <param name="growlDto">The DTO with the data of the current Growl</param>
	public void SetUpGrowl(GrowlDTO growlDto)
	{
		_growlDto = growlDto;
	}

	/// <summary>
	///		Activates the Growl area and checks collision against NPCs
	/// </summary>
	public void ActivateGrowlArea()
	{
		((CircleShape2D)_collisionShape.Shape).Radius = _growlDto.Radius;
		_collisionShape.Disabled = false;

		BodyEntered += OnBodyEntered;
	}

	private void OnBodyEntered(Node2D body)
	{
		if (body is Npc enemy)
		{
			var offset = _player.SpriteSize.Y / 2;

			var raycast = new RayCast2D()
			{
				Position = new Vector2(0f, -offset),
				TargetPosition = enemy.GlobalPosition - _player.GlobalPosition,
				CollisionMask = (int)CollisionLayerName.World
			};

			_player.AddChild(raycast);
			raycast.ForceRaycastUpdate();
			enemy.ReactToGrowl(_growlDto.GrowlType);
			raycast.QueueFree();
		}
	}

	public void DeactivateGrowlArea()
	{
		var collisionShape = GetChild(0) as CollisionShape2D;

		collisionShape.Disabled = true;

		((CircleShape2D)collisionShape.Shape).Radius = 0f;

		_growlDto = null;

		BodyEntered -= OnBodyEntered;
	}
}
