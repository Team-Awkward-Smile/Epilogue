using Epilogue.global.enums;
using Godot;

namespace Epilogue.props.breakable_tile;
/// <summary>
///		A static sprite that can detect damage caused to it and be destroyed.
///		Meant to look like a regular tile at first glance
/// </summary>
public partial class BreakableTile : StaticBody2D
{
	[Export] private float _health;

	private Sprite2D _sprite;

	/// <inheritdoc/>
	public override void _Ready()
	{
		_sprite = GetNode<Sprite2D>("Sprite2D");
	}

	public async void DamageTile(float damage, DamageType damageType)
	{
		if(damageType == DamageType.Unarmed)
		{
			damage /= 2;
		}

		if((_health -= damage) <= 0)
		{
			QueueFree();
		}

		var rng = new RandomNumberGenerator();

		for(var i = 0; i < 3; i++)
		{
			_sprite.Position = new Vector2(rng.RandiRange(-1, 1), rng.RandiRange(-1, 1));

			await ToSignal(GetTree().CreateTimer(0.05f), "timeout");

			_sprite.Position = Vector2.Zero;

			await ToSignal(GetTree().CreateTimer(0.05f), "timeout");
		}
	}
}
