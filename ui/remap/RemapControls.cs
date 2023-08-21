using Epilogue.nodes;

using System.Collections.Generic;

namespace Epilogue.ui;
public partial class RemapControls : UI
{
	private readonly Dictionary<string, List<string>> _moveActions = new()
	{
		{ "Move Left", new() { "move_left" } },
		{ "Move Right", new() { "move_right" } },
		{ "Run", new() { "toggle_run" } },
		{ "Slide", new() { "slide" } },
		{ "Cancel Slide/Look Up", new() { "cancel_slide", "look_up" } },
		{ "Crouch/Look Down", new() { "crouch" } },
		{ "Jump/Vault/Climb Ledge", new() { "jump" } }
	};

	private readonly Dictionary<string, List<string>> _combatActions = new()
	{
		{ "Attack/Shoot", new() { "melee", "shoot" } },
		{ "Growl/Interact", new() { "growl", "interact" } }
	};

	private readonly Dictionary<string, List<string>> _uiActions = new()
	{
		{ "Pause/Unpause", new() { "pause" } }
	};

	public override void _Ready()
	{
		GetNode<RemapAction>("ScrollContainer/Control/MovementActions").AddActionRow(_moveActions);
		GetNode<RemapAction>("ScrollContainer/Control/CombatActions").AddActionRow(_combatActions);
		GetNode<RemapAction>("ScrollContainer/Control/UIActions").AddActionRow(_uiActions);
	}
}
