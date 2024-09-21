using Epilogue.Nodes;
using Godot;
using System;


public partial class Spike : RigidBody2D
{
	private RayCast2D _rayCast2D;
	private bool _dropped = false;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_rayCast2D = GetNode<RayCast2D>("RayCast2D");
		
		TileMap levelTileMap = (TileMap)GetParent();
		levelTileMap.AddToLstSpike(this);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override async void _Process(double delta)
	{
		if (_rayCast2D.GetCollider() is Player && !_dropped)
		{
			_dropped = true;
			GravityScale = 1;
			await ToSignal(GetTree().CreateTimer(3), "timeout");
			QueueFree();
		}

	}	

	public void setRaycatDistance(int distance)
	{
		Vector2 newDistance = new Vector2(_rayCast2D.TargetPosition.X, distance * 36);
		_rayCast2D.TargetPosition = newDistance;
	}
}
