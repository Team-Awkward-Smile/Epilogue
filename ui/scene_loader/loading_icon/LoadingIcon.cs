using Godot;
using System;

public partial class LoadingIcon : Sprite2D
{
	AnimationPlayer animation_player;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		animation_player = GetNode<AnimationPlayer>("AnimationPlayer");
		animation_player.Play("Loading");
	}

	public void _on_timer_timeout() 
	{
		Hide();
	}
}
