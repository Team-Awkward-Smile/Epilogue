using Epilogue.Nodes;
using Godot;
using System;
using System.Linq.Expressions;

[Icon("res://tile_map/scenes/spikes/v1/Spikes2.png")]
public partial class Spike : RigidBody2D
{

	// export variable for crumbs amount
	[Export] public int RubbleAmount = 10;
	[Export] public PackedScene RubbleScene;
	[Export] public float LifeTime = 10f;

	private RayCast2D _trigger;
	private RayCast2D _leftWarning;
	private RayCast2D _rightWarning;
	private AnimationPlayer _animationPlayer;
	private GpuParticles2D _gpuParticles2D;
	private Node2D _rubbles;
	
	private float _initPosY;

	private bool _dropped = false;
	private bool _hitFloor = false;
	private float _triggerHeight;
	private bool _warned = false;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		// Getting the initial nodes
		_initPosY = Position.Y;
		_trigger = GetNode<RayCast2D>("Trigger");
		_leftWarning = GetNode<RayCast2D>("LeftWarning");
		_rightWarning = GetNode<RayCast2D>("RightWarning");

		_animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
		_gpuParticles2D = GetNode<GpuParticles2D>("GPUParticles2D");
		_gpuParticles2D.Lifetime = LifeTime;
		
		_rubbles = GetNode<Node2D>("Rubbles");
		
		// Adding the spike to the list of spikes in the levelTileMap
		TileMap levelTileMap = (TileMap)GetParent();
		levelTileMap.AddToLstSpike(this);
		AddRebbles();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override async void _Process(double delta)
	{	
		// If the player is in the trigger area, drop the spike
		if (_trigger.GetCollider() is Player && !_dropped)
		{
			_dropped = true;
			GravityScale = 1;
		}
		
		// If the spike has dropped and hit the floor, start emitting particles
		if (Math.Abs(_initPosY - Position.Y) > _triggerHeight && !_hitFloor)
		{
			_hitFloor = true;
			_gpuParticles2D.Emitting = true;
			GetNode<Sprite2D>("Sprite2D").QueueFree();

			_rubbles.Show();
			foreach (Node2D rubble in _rubbles.GetChildren())
			{
				((RigidBody2D)rubble).Freeze = false;
			}
		}

		// If the player is in the warning area, play the warning animation
		if ((_leftWarning.GetCollider() is Player || _rightWarning.GetCollider() is Player) && !_warned)
		{
			_animationPlayer.Play("twitch");
			_warned = true;
		}
		
	}	

	// Set the distance of the raycast, its used to calculate the distance between the spike and the floor
	public void setRaycatDistance(int distance)
	{
		Vector2 newDistance = new Vector2(0, distance * 36);
		_triggerHeight = newDistance.Y - 18;
		_trigger.TargetPosition = newDistance;
		_leftWarning.TargetPosition = newDistance;
		_rightWarning.TargetPosition = newDistance;
	}

	// Add the crumbs to the spike
	private void AddRebbles()
	{
		for (int i = 0; i < RubbleAmount; i++)
		{
			Node2D new_rubble = (Node2D)RubbleScene.Instantiate();
			_rubbles.AddChild(new_rubble);
		}
		
	}

	// Called when the spike has finished emitting particles
	public void _on_gpu_particles_2d_finished()
	{
		QueueFree();
	}

}
