using System.Collections.Generic;
using System.Linq;
using Epilogue.nodes;
using Godot;

public partial class QuickSand : Area2D
{
    [Export] private float _maxSlowPercentage;

    private List<Actor> _actors = new();
    private float _timer = 0f;
    private float _defaultGravity;
    private float _height;

	public override void _Ready()
	{
        var shape = GetChildren().OfType<CollisionShape2D>().First().Shape as RectangleShape2D;

        _defaultGravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();
        _height = shape.Size.Y;

        BodyEntered += StartSinking;
        BodyExited += ResetSinking;
	}

    private void StartSinking(Node2D body)
    {
        var actor = body as Actor;

        actor.Gravity = Gravity;
        actor.Conditions |= Conditions.Sinking;
        actor.FloorSnapLength = 0f;

        _actors.Add(actor);
    }

	public override void _PhysicsProcess(double delta)
	{
        // Only updates the Gravity of the Actors once every 0.1 second (or about once every 6 frames)
        if(((_timer += (float) delta) < 0.1f) || !_actors.Any())
        {
            return;
        }

        _timer = 0f;

        foreach(var actor in _actors)
        {
            var slowPercentage = ((actor.GlobalPosition.Y - GlobalPosition.Y) / _height) * _maxSlowPercentage;

            if(_maxSlowPercentage - slowPercentage < 0.02f)
            {
                slowPercentage = _maxSlowPercentage;
            }

            actor.SlowWeight = slowPercentage;
        }
	}
	
    private void ResetSinking(Node2D body)
    {
        var actor = body as Actor;
        
        actor.SlowWeight = 0f;
        actor.Gravity = _defaultGravity;
        actor.Conditions &= ~Conditions.Sinking;
        actor.FloorSnapLength = 5f;

        _actors.Remove(actor);
    }
}
