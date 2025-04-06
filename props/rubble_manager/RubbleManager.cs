using Godot;
using System;
using System.Dynamic;


public partial class RubbleManager : Node2D
{
	[Export] private PackedScene _rubbleScene; 

	/// <summary>
	/// Update the number of rubble contained in the RubbleManager
	/// </summary>
	private int _amount;

	public void SetRubbleAmount(int nbr)
	{
		_amount = nbr;
		_updateRubble();
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

		for (int i = 0; i < _amount; i++)
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
			Child.Freeze = false;
			Child.LinearVelocity = new Vector2(
				GD.RandRange(-100, 100),
				GD.RandRange(-100, -200)
			);
		}
	}
}
