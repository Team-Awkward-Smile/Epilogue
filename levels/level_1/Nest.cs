using Epilogue.nodes;
using Godot;
using System;

public partial class Nest : TileMap
{
    [Export] private float _maxRotationDegrees;

	private Rect2I _usedArea;
    private float _left;
    private float _right;
    private float _middle;
    private float _lengthLeft;
    private float _lengthRight;
    private float _currentAngle;
    private Player _player;
    private bool _canRotate;
    private bool _canTween;
    private SegmentShape2D _bridgeCollision;
    private Line2D _bridgeSprite;
    private Marker2D _marker;


	public override void _Ready()
	{
		_usedArea = GetUsedRect();
        _left = MapToLocal(_usedArea.Position).X;
        _right = MapToLocal(_usedArea.End).X;
        _middle = (_right + _left) / 2;
        _lengthLeft = _left - _middle;
        _lengthRight = _right - _middle;
        _player = GetTree().GetLevel().Player;
        _bridgeCollision = GetNode<CollisionShape2D>("../Bridge/CollisionShape2D").Shape as SegmentShape2D;
        _bridgeSprite = GetNode<Line2D>("../Bridge/Line2D");
        _marker = GetNode<Marker2D>("Marker2D");

        var area = GetNode<Area2D>("Area2D");

        area.BodyEntered += (Node2D body) => 
        {
            GetTree().CreateTimer(0.1f).Timeout += () => 
            {
                _canTween = true;
                _canRotate = true;
            };
        };

        area.BodyExited += (Node2D body) => _canRotate = false;
	}

	public override void _PhysicsProcess(double delta)
	{
		if(_canRotate)
        {
            var position = _player.GlobalPosition.X - _middle;
            var weight = position / (position > 0f ? _lengthRight : _lengthLeft);

            _currentAngle = _maxRotationDegrees * weight * (position < 0f ? -1 : 1);
        }
        else
        {
            _currentAngle = 0f;
            _canTween = true;
        }

        if(_canTween)
        {
            GetTree().CreateTween().TweenProperty(this, "rotation_degrees", _currentAngle, 0.2f);
            _canTween = false;
        }
        else
        {
            RotationDegrees = _currentAngle;
        }

        _bridgeCollision.B = _marker.GlobalPosition;
        _bridgeSprite.SetPointPosition(1, new Vector2(_marker.GlobalPosition.X - 5f, _marker.GlobalPosition.Y + 5f));
	}
}
