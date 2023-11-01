using Epilogue.extensions;
using Epilogue.nodes;

using Godot;

public partial class Die : NpcState
{
	internal override void OnEnter(params object[] args)
	{
		Npc.Sprite.SetShaderMaterialParameter("iframeActive", false);
		AnimPlayer.PlayBackwards("Combat/die");
		AnimPlayer.AnimationFinished += (StringName animName) => Npc.QueueFree();
	}
}
