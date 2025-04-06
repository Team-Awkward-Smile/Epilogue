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

	private bool _killSwitchActive = false;
	

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_rubbles = GetNode<Node2D>("Rubbles");
	}

	public void _on_spawn_body_entered(Node body)
	{
		
		GD.Print("Spawning Rubbles");
		active = true;
		
	}

	public void _on_despawn_body_entered(Node body)
	{

		GD.Print("Despawning Rubbles");
	}


}

