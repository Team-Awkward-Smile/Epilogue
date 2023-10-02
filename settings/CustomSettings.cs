using Godot;
using System;

namespace Epilogue.settings;
/// <summary>
///		Script to set up custom settings in the Project Settings tab
/// </summary>
[Tool]
public partial class CustomSettings : Node
{
	/// <inheritdoc/>
	public override void _Ready()
	{
		ProjectSettings.AddPropertyInfo(new Godot.Collections.Dictionary()
		{
			{ "name", "epilogue/controls/controller_type" },
			{ "type", (int) Variant.Type.Int },
			{ "hint", (int) PropertyHint.Enum },
			{ "hint_string", "PlayStation,XBox,Nintendo Switch" }
		});

		ProjectSettings.AddPropertyInfo(new Godot.Collections.Dictionary()
		{
			{ "name", "epilogue/gameplay/game_cycle" },
			{ "type", (int) Variant.Type.Int },
			{ "hint", (int) PropertyHint.Enum },
			{ "hint_string", "New Game,New Game+" }
		});

		ProjectSettings.AddPropertyInfo(new Godot.Collections.Dictionary()
		{
			{ "name", "epilogue/controls/control_scheme" },
			{ "type", (int) Variant.Type.Int },
			{ "hint", (int) PropertyHint.Enum },
			{ "hint_string", "Modern,Retro" }
		});
	}
}
