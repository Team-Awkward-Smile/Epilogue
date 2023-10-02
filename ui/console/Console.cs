using Godot;
using System;

namespace Epilogue.ui.console;
/// <summary>
///		Console Window to run debugging commands during runtime
/// </summary>
public partial class Console : Window
{
	private LineEdit _textInput;
	private RichTextLabel _log;
	private Commands _commands;

	/// <inheritdoc/>
	public override void _Ready()
	{
		CloseRequested += Hide;

		_textInput = GetNode<LineEdit>("TextInput");
		_log = GetNode<RichTextLabel>("%Log");
		_textInput.TextSubmitted += (string text) => ExecuteCommand(text.Trim());
		_commands = new Commands();

		FocusEntered += () =>
		{
			_textInput.GrabFocus();
		};

		AddChild(_commands);
	}

	private void ExecuteCommand(string command)
	{
		_textInput.Clear();

		var args = command.Split(' ');
		var result = _commands.ExecuteCommand(args);
		var color = result.Status == CommandResultStatus.Success 
			? "[color=white]" 
			: result.Status == CommandResultStatus.Warning ? "[color=yellow]" : "[color=red]";

		_log.AppendText($"{color}[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] - {result.Response}[/color]\n");
	}
}
