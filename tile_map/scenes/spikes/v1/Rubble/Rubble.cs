using Epilogue.Nodes;
using Godot;
using System;
using System.Collections.Generic;

public partial class Rubble : RigidBody2D
{
    [Export] public float SlowMultipler = 5f;
	// TODO a set get that updates 
	public int RubblesNeighborsCount => RubblesNeighborsList.Count;
    public List<Rubble> RubblesNeighborsList = new List<Rubble>();

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        // Initialization code here
    }

    public void _on_area_2d_body_entered(Node2D node)
    {
        if (node is Rubble rubble)
        {
            if (!RubblesNeighborsList.Contains(rubble))
            {
                RubblesNeighborsList.Add(rubble);
            }
        }

        if (node is Player player)
        {
            // Add logic for player interaction if needed
        }
    }

    public void _on_area_2d_body_exited(Node2D node)
    {
        if (node is Rubble rubble)
        {
            if (RubblesNeighborsList.Contains(rubble))
            {
                RubblesNeighborsList.Remove(rubble);
            }
        }

        if (node is Player player)
        {
            // Add logic for player interaction if needed
        }
    }

    // Override the _Notification method to detect when the node is about to be freed
    public override void _Notification(int what)
    {
        if (what == NotificationPredelete)
        {
            OnQueueFree();
        }
    }

    // Custom function to be called when the node is about to be freed
    private void OnQueueFree()
    {
        // Add your custom logic here
        GD.Print("Rubble node is about to be freed.");
    }
}

