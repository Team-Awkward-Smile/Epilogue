using Godot;
using System.Collections.Generic;

namespace Epilogue.nodes;
[GlobalClass]
public partial class KonamiCode : Node
{
	[Export] private float _cheatInputTimer = 1f;

	private float _timer = 0f;
	private List<InputEvent> _cheatBuffer = new();

	private static string[] _cheat = { 
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

	public override void _UnhandledInput(InputEvent @event)
	{
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

			GD.Print("Konami Code activated");
			QueueFree();
		}
	}
}
