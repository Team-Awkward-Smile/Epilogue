using Epilogue.extensions;
using Epilogue.nodes;

using Godot;
using System;

namespace Epilogue.actors.rob.states;
/// <summary>
///		State to allow Rob to get stunned and stop acting
/// </summary>
public partial class Stun : NpcState
{
	internal override void OnEnter()
	{
		AnimPlayer.PlayBackwards("Combat/stun");

		Npc.Sprite.SetShaderMaterialParameter("iframeActive", true);
		Npc.CanProcessAI = false;
	}

	internal override void OnLeave()
	{
		Npc.CanProcessAI = true;
	}
}
