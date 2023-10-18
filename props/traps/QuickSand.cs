using Epilogue.nodes;
using Godot;
using System;
using System.Linq;

public partial class QuickSand : Area2D
{
    [Export] private float _sinkTime;
    [Export] private Vector2 _finalPosition;

    private CollisionShape2D _poolCollision;
    private Tween _tween;

	public override void _Ready()
	{
        _poolCollision = GetChildren().OfType<AnimatableBody2D>().First().GetChild(0) as CollisionShape2D;

        BodyEntered += StartSinking;
        BodyExited += ResetSinking;
	}

    private void StartSinking(Node2D body)
    {
        ((Actor) body).IsTrapped = true;

        _tween = GetTree().CreateTween();

        _tween.TweenProperty(_poolCollision, "position", _finalPosition, _sinkTime);
    }

    private void ResetSinking(Node2D body)
    {
        ((Actor) body).IsTrapped = false;

        if(_tween.IsValid())
        {
            _tween.Kill();
        }

        _poolCollision.Position = Vector2.Zero;
    }
}
