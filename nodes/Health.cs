using Epilogue.global.enums;
using Godot;
using System.Linq;

namespace Epilogue.nodes;
/// <summary>
///		Node for controlling an Actor's HP
/// </summary>
[GlobalClass]
public partial class Health : Node
{
	[Export] private float MaxHealth { get; set; }

	public float CurrentHealth 
	{ 
		get => _currentHealth;
		set
		{
			_currentHealth = value;
			UpdateHealthSprite();
		} 
	}

	protected Actor _actor;

	private float _currentHealth;
	private PackedScene _heartSprite;
	private HBoxContainer _heartContainer;

    public override void _Ready()
	{
		_actor = (Actor) Owner;
		_heartSprite = GD.Load<PackedScene>("res://temp/heart_sprite.tscn");
		_heartContainer = new HBoxContainer();

		_heartContainer.AddThemeConstantOverride("separation", 15);

		AddChild(_heartContainer);

		CurrentHealth = MaxHealth;
	}

	private void UpdateHealthSprite()
	{
		_heartContainer.GetChildren().ToList().ForEach(c => c.QueueFree());

		var temp = CurrentHealth;

		for(var i = 0; i < MaxHealth; i++)
		{
			var heart = _heartSprite.Instantiate() as Control;

			_heartContainer.AddChild(heart);

			((Sprite2D) heart.GetChild(0)).Frame = temp > 0 ? temp switch 
			{
				0.25f => 3,
				0.5f => 2,
				0.75f => 1,
				_ => 0
			} : 4;

			temp--;
		}
	}

	public override void _PhysicsProcess(double delta)
	{
		_heartContainer.GlobalPosition = _actor.GlobalPosition - new Vector2(_heartContainer.Size.X / 2, _actor.Sprite.GetRect().Size.Y * _actor.Sprite.Scale.Y);
	}

	public virtual void DealDamage(float damage)
	{
		if(damage < 0)
		{
			GD.PushWarning($"Trying to deal negative damage to Actor [{_actor.Name}]. Use ApplyHealth instead");
			return;
		}

		CurrentHealth -= damage;

		if(CurrentHealth <= 0)
		{
			_actor.QueueFree();
		}

	}

	public virtual void ApplyHealth(float health) 
	{
		if(health < 0)
		{
			GD.PushWarning($"Trying to apply negative health to Actor [{_actor.Name}]. Use DealDamage instead");
			return;
		}

		CurrentHealth += health;
	}
}
