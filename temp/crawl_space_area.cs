using Epilogue.Nodes;
using Godot;
using System;

public partial class crawl_space_area : Area2D
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	public void _on_body_entered(Node2D body)
	{	
		if (body is Gun)
		{
			Gun gun = (Gun)body;
			gun.SelfDestruct();
		}

	}
}
