using Epilogue.Nodes;
using Godot;
using System;
using System.Collections.Generic;

public partial class Rubble : RigidBody2D
{
	// TODO a set get that updates 
	private int RubblesNeighborsCount => RubblesNeighborsList.Count;
    private List<Rubble> RubblesNeighborsList = new List<Rubble>();

	private Player _player;

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
            player.RubblesNeighborsList.Add(this);
			_player = player;

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
            player.RubblesNeighborsList.Remove(this);
			_player = null;
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
		if (RubblesNeighborsList.Count > 0)
		{
			foreach (var rubble in RubblesNeighborsList)
			{
				rubble.RubblesNeighborsList.Remove(this);
			}
		}
		if (_player != null){
			_player.RubblesNeighborsList.Remove(this);
		}
    }
}

