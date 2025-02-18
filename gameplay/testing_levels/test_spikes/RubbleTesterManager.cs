using Godot;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Threading.Tasks;

public partial class RubbleTesterManager : Node2D
{


	[Export] private PackedScene _rubbleScene;
	[Export] private int _maxRubbles = 100;
	private Node2D _rubbles;
	private bool active = false;
	

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_rubbles = GetNode<Node2D>("Rubbles");
	}

	/// <inheritdoc/>

	private void SpawnRubble()
	{
		if (active && _rubbles.GetChildCount() < _maxRubbles)
		{
			Rubble rubble = _rubbleScene.Instantiate<Rubble>();
			rubble.Freeze = false;
			_rubbles.AddChild(rubble);		
		}
	}

	public void _on_spawn_body_entered(Node body)
	{
		
		GD.Print("Spawning Rubbles");
		active = true;
		
	}

	public void _on_despawn_body_entered(Node body)
	{

		GD.Print("Despawning Rubbles");
		active = false;
		if (_rubbles.GetChildCount() > 0)
		{
			foreach (var rubble in _rubbles.GetChildren())
			{
				rubble.QueueFree();
			}
		}
	}

	public void _on_timer_timeout()
	{
		SpawnRubble();
	}

}

