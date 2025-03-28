using Epilogue.Nodes;
using Godot;
using System;
using System.Linq.Expressions;

[Icon("res://tile_map/scenes/spikes/v1/Spikes2.png")]
public partial class Spike : RigidBody2D
{

	// TODO : 
	// - Random chance of falling 	
	// - If it doesnt fall, shake for warning
	// - Try collision

	// export variable for crumbs amount
	[Export] public float _lifeTime = 10f;
	[Export] public int RubbleAmount = 1;
	private RayCast2D _trigger;
	private RayCast2D _leftWarning;
	private RayCast2D _rightWarning;
	private AnimationPlayer _animationPlayer;
	private GpuParticles2D _gpuParticles2D;
	private Timer _timer;
	private RubbleManager _rubbleManager;
	private float _initPosY;

	private bool _dropped = false;
	private bool _playerInside = false;
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
		_gpuParticles2D = GetNode<GpuParticles2D>("Dust");
		_timer = GetNode<Timer>("Timer");
		
		_rubbleManager = GetNode<RubbleManager>("RubbleManager");
		_rubbleManager.Amount = RubbleAmount;

		_timer.WaitTime = _lifeTime;
		
		// Adding the spike to the list of spikes in the levelTileMap
		TileMap levelTileMap = (TileMap)GetParent();
		levelTileMap.AddToLstSpike(this);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override async void _Process(double delta)
	{	
		// If the player is in the trigger area, drop the spike
		if (_trigger.GetCollider() is Player == false)
		{
			_playerInside = false;
		}

		if (_trigger.GetCollider() is Player && !_dropped && !_playerInside)
		{
			_playerInside = true;

			double odds = GD.RandRange(0,1);
			GD.Print(odds);
			if (odds == 1)
			{
				_dropped = true;
				GravityScale = 1;
				_rubbleManager.Activate();
			}
			else
			{
				_animationPlayer.Play("twitch");
			}
		}
		
		// If the spike has dropped and hit the floor, start emitting particles
		if (Math.Abs(_initPosY - Position.Y) > _triggerHeight && !_hitFloor)
		{
			_hitFloor = true;
			_gpuParticles2D.Emitting = true;
			GetNode<Sprite2D>("Sprite2D").QueueFree();

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

	// Called when the spike has finished emitting particles
	public void _on_gpu_particles_2d_finished()
	{
		QueueFree();
	}

}
