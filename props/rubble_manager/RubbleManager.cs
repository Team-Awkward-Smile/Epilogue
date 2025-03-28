using Godot;
using System;
using System.Dynamic;


public partial class RubbleManager : Node2D
{
	[Export] private PackedScene _rubbleScene; 

	/// <summary>
	/// Update the number of rubble contained in the RubbleManager
	/// </summary>
	public int Amount {
		set{Amount = value; _updateRubble();}
		get{return Amount;}
	}
	private void _updateRubble()
	{
		if (GetChildCount() > 0)
		{
			foreach (var child in GetChildren())
			{
				child.QueueFree();
			}
		}

		for (int i = 0; i < Amount; i++)
		{
			AddChild(_rubbleScene.Instantiate());
		}
	
	
	}

	public void Activate()
	{
		Show();
		foreach (RigidBody2D Child in GetChildren())
		{
			Child.Sleeping = false;
		}
	}
}
