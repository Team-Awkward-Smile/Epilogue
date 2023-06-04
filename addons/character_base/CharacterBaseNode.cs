#if TOOLS
using Godot;

namespace Epilogue.addons;
[Tool]
public partial class CharacterBaseNode : EditorPlugin
{
	public override void _EnterTree()
	{
		var script = GD.Load<Script>("res://addons/character_base/CharacterBase.cs");
		var texture = GD.Load<Texture2D>("res://test.png");

		AddCustomType("CharacterBase", "CharacterBody2D", script, texture);
	}

	public override void _ExitTree()
	{
		RemoveCustomType("CharacterBase");
	}
}
#endif
