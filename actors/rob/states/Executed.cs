using Epilogue.extensions;
using Epilogue.nodes;

using Godot;

public partial class Executed : NpcState
{
	internal override void OnEnter()
	{
		Npc.Sprite.SetShaderMaterialParameter("iframeActive", false);

		AnimPlayer.PlayBackwards("Combat/execution");
		AnimPlayer.AnimationFinished += (StringName animName) =>
		{
			GetTree().CreateTimer(2f).Timeout += () =>
			{
				Npc.QueueFree();
			};
		};
	}
}
