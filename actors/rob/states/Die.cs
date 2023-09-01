using Epilogue.nodes;

using Godot;
using System;

public partial class Die : NpcState
{
	internal override void OnEnter()
	{
		AnimPlayer.PlayBackwards("Combat/die");
		AnimPlayer.AnimationFinished += (StringName animName) => GetParent().QueueFree();
	}
}
