using Godot;
using System.Collections.Generic;

namespace Epilogue.Nodes;
/// <summary>
///		Node used to read inputs from the player and unlocking secrets when the Konami Code is read
/// </summary>
[GlobalClass]
public partial class KonamiCode : Node
{
	[Export] private float _cheatInputTimer = 1f;

	private float _timer = 0f;
	private readonly List<InputEvent> _cheatBuffer = new();

	// Predefined cheat: up up down down left right left right RMB LMB Enter
	private static readonly string[] _cheat = { 
		"cheat_up", 
		"cheat_up", 
		"cheat_down", 
		"cheat_down", 
		"cheat_left", 
		"cheat_right", 
		"cheat_left", 
		"cheat_right",
		"cheat_button_1",
		"cheat_button_2",
		"cheat_button_3"
	};

	/// <inheritdoc/>
	public override void _UnhandledInput(InputEvent @event)
	{
		// Ignores any input that's not from the Keyboard/Mouse or from the D-Pad
		if((@event is not (InputEventKey or InputEventJoypadButton or InputEventMouseButton)) || @event.IsReleased())
		{
			return;
		}

		_timer = 0f;
		_cheatBuffer.Add(@event);

		if(_cheatBuffer.Count > _cheat.Length)
		{
			_cheatBuffer.Clear();
		}
	}

	/// <inheritdoc/>
	public override void _Process(double delta)
	{
		_timer += (float) delta;

		if(_timer >= _cheatInputTimer)
		{
			_cheatBuffer.Clear();
		}

		if(_cheatBuffer.Count == _cheat.Length)
		{
			var i = 0;

			foreach(var input in _cheatBuffer)
			{
				if(!input.IsAction(_cheat[i++]))
				{
					_cheatBuffer.Clear();
					return;
				}
			}

			// TODO: actually unlock stuff when the code is entered correctly
			GD.Print("Konami Code activated");
			QueueFree();
		}
	}
}
