using Epilogue.nodes;
using Godot;
using System;
using System.Linq;

public partial class QuickSand : Area2D
{
    [Export] private float _sinkTime;
    [Export] private Vector2 _finalPosition;
    [Export] private float _maxSlowPercentage;
    [Export] private float _maxSlowTime;

    private CollisionShape2D _poolCollision;
    private Tween _positionTween;
    private Tween _slowTween;
    private float _slowWeight = 0f;
    private bool _isSinking = false;
    private Actor _actor;

	public override void _Ready()
	{
        _poolCollision = GetChildren().OfType<AnimatableBody2D>().First().GetChild(0) as CollisionShape2D;

        BodyEntered += StartSinking;
        BodyExited += ResetSinking;
	}

    private void StartSinking(Node2D body)
    {
        _isSinking = true;
        _actor = (Actor) body;

        _positionTween = GetTree().CreateTween();
        _slowTween = GetTree().CreateTween();

        _positionTween.TweenProperty(_poolCollision, "position", _finalPosition, _sinkTime);
        _slowTween.TweenProperty(this, "_slowWeight", _maxSlowPercentage, _maxSlowTime);
    }

	public override void _PhysicsProcess(double delta)
	{
		if(!_isSinking)
        {
            return;
        }

        _actor.SlowWeight = _slowWeight;
	}
	
    private void ResetSinking(Node2D body)
    {
        _isSinking = false;
        _actor.SlowWeight = 0f;
        _slowWeight = 0f;

        if(_positionTween.IsValid())
        {
            _positionTween.Kill();
        }

        if(_slowTween.IsValid())
        {
            _slowTween.Kill();
        }

        _poolCollision.Position = Vector2.Zero;
    }
}
