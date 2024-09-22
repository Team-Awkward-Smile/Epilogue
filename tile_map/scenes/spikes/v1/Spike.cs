using Epilogue.Nodes;
using Godot;
using System;

[Icon("res://tile_map/scenes/spikes/v1/Spikes2.png")]
public partial class Spike : RigidBody2D
{
	private RayCast2D _trigger;
	private RayCast2D _leftWarning;
	private RayCast2D _rightWarning;

	private AnimationPlayer _animationPlayer;
	private bool _dropped = false;
	private bool _warned = false;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_trigger = GetNode<RayCast2D>("Trigger");
		_leftWarning = GetNode<RayCast2D>("LeftWarning");
		_rightWarning = GetNode<RayCast2D>("RightWarning");

		_animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
		
		TileMap levelTileMap = (TileMap)GetParent();
		levelTileMap.AddToLstSpike(this);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override async void _Process(double delta)
	{
		if (_trigger.GetCollider() is Player && !_dropped)
		{
			_dropped = true;
			GravityScale = 1;
			await ToSignal(GetTree().CreateTimer(3), "timeout");
			QueueFree();
		}

		if ((_leftWarning.GetCollider() is Player || _rightWarning.GetCollider() is Player) && !_warned)
		{
			_animationPlayer.Play("twitch");
			_warned = true;
		}

	}	

	public void setRaycatDistance(int distance)
	{
		Vector2 newDistance = new Vector2(0, distance * 36);
		_trigger.TargetPosition = newDistance;
		_leftWarning.TargetPosition = newDistance;
		_rightWarning.TargetPosition = newDistance;
	}
}
