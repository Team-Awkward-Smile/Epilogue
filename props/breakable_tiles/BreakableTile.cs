using Epilogue.Global.Enums;
using Godot;
using Godot.Collections;
using System.Linq;

namespace Epilogue.Props.BreakableTile;
/// <summary>
///		A static sprite that can detect damage caused to it and be destroyed.
///		Meant to look like a regular tile at first glance
/// </summary>
public partial class BreakableTile : StaticBody2D
{
    [Export] private float _health;
    [Export] private Array<AudioStreamWav> _breakingSounds;

    private Sprite2D _sprite;
    private AudioStreamPlayer2D _audioPlayer;
    private AnimationPlayer _animationPlayer;

    /// <inheritdoc/>
    public override void _Ready()
    {
        _sprite = GetNode<Sprite2D>("Sprite2D");
        _audioPlayer = GetNode<AudioStreamPlayer2D>("AudioStreamPlayer2D");
        _animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
    }

    /// <summary>
    /// 	Damages this tile. If the damage reduces it's HP to 0 or less, the tile will be destroyed
    /// </summary>
    /// <param name="damage">Amount of damage caused to this tile</param>
    /// <param name="damageType">Type of damage of the damaging attack. Unarmed attacks deal half damage to tiles</param>
    public async void DamageTile(float damage, DamageType damageType)
    {
        if (damageType == DamageType.Unarmed)
        {
            damage /= 2;
        }

        var rng = new RandomNumberGenerator();

        if ((_health -= damage) <= 0)
        {
            CollisionLayer = 0;
            CollisionMask = 0;

            var sfx = _breakingSounds.ElementAt(rng.RandiRange(0, _breakingSounds.Count - 1));

            _audioPlayer.Stream = sfx;
            _audioPlayer.Play();
            _animationPlayer.Play("break");

            _animationPlayer.AnimationFinished += (StringName animName) => QueueFree();

            return;
        }

        for (var i = 0; i < 3; i++)
        {
            _sprite.Position = new Vector2(rng.RandiRange(-1, 1), rng.RandiRange(-1, 1));

            await ToSignal(GetTree().CreateTimer(0.05f), "timeout");

            _sprite.Position = Vector2.Zero;

            await ToSignal(GetTree().CreateTimer(0.05f), "timeout");
        }
    }
}
