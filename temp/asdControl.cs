using Godot;
using System;

public partial class asdControl : Label
{
	public override void _Input(InputEvent @event)
	{
		if(@event is InputEventKey key && @event.IsPressed())
		{
			Text = "Label: " + key.KeyLabel + $"\nUnicode: [{key.Unicode}]" + "\nUnicode Char: " + ((char) key.Unicode).ToString();
		}
	}
}
