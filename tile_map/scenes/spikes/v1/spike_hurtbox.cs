using Epilogue.Global.Enums;
using Epilogue.Nodes;
using Godot;
using System;
public partial class spike_hurtbox : HitBox
{	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{

	}

	public void _on_area_entered(Area2D area2D)
	{
		if (area2D is HurtBox hurtBox)
		{
			hurtBox.OnHit(this);
		}
		// Monitoring = false;	
	}
}