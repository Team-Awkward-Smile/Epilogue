#if TOOLS
using Godot;
using System;

[Tool]
public partial class CustomNode : EditorPlugin
{
	public override void _EnterTree()
	{
		var script = GD.Load<Script>("res://addons/base_scene/BaseScene.cs");
        var texture = GD.Load<Texture2D>("res://test.png");

        AddCustomType("BaseScene", "Node2D", script, texture);
	}

	public override void _ExitTree()
	{
		RemoveCustomType("BaseScene");
	}
}
#endif