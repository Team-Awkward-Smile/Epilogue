using Epilogue.nodes;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Epilogue.ui.console;
/// <summary>
///		List of all available commands that can be executed from the Console
/// </summary>
public partial class Commands : Node
{
	private Dictionary<string, Func<string[], CommandResponse>> _commands;

	/// <inheritdoc/>
	public override void _Ready()
	{
		_commands = new()
		{
			{ "help", (args) => Help() },
			{ "current_zoom", (args) => CurrentZoom() },
			{ "$", (args) => GetNode(args) }
		};
	}

	public CommandResponse ExecuteCommand(string[] args)
	{
		if(_commands.ContainsKey(args[0]))
		{
			var command = args[0];
			var arguments = args[1..];

			return _commands[command].Invoke(arguments);
		}
		else
		{
			return new CommandResponse()
			{
				Status = CommandResultStatus.Success,
				Response = $"Invalid command: [{args[0]}]"
			};
		}
	}

	private CommandResponse Help()
	{
		return new CommandResponse()
		{
			Status = CommandResultStatus.Success,
			Response = "Printing help"
		};
	}

	private CommandResponse CurrentZoom()
	{
		var camera = GetTree().Root.GetNode<Level>("Root").Camera;

		return new CommandResponse()
		{
			Status = CommandResultStatus.Success,
			Response = $"Current Zoom: [{camera.Zoom}]"
		};
	}

	private CommandResponse GetNode(string[] args)
	{
		try
		{
			var node = GetNode($"/root/{(args.Length > 0 ? args[0] : "")}");
			var status = CommandResultStatus.Success;
			var response = "";

			switch(args.Length)
			{
				// 1) [<Node>] OR 2) []
				// 1) Prints every children of <Node>, or 2) every children of /root/
				case < 2:
					response = PrintChildren(node);
					break;

				// 1) [<Node> <property>] OR 2) [<Node> .]
				// 1) Prints the value of <property>, or 2) all properties of <Node>
				case 2:
					if(args[1] == ".")
					{
						response = PrintProperties(node);
					}
					else
					{
						var property = node.Get(args[1]);

						status = property.VariantType != Variant.Type.Nil ? CommandResultStatus.Success : CommandResultStatus.Warning;
						response = property.VariantType != Variant.Type.Nil ? $"[{args[1]}] - [{property}]" : $"Property [{args[1]}] not found for Node [{node.Name}]";
					}
					break;

				// [<Node> <property> <value>]
				// Sets the value of <property> to <value>
				case 3:
					var p = node.Get(args[1]);
					var type = p.VariantType;

					node.Set(p.AsStringName(), args[2]);

					response = $"Setting [{args[1]}] to [{args[2]}]";

					break;
			}

			return new CommandResponse()
			{
				Status = status,
				Response = response
			};
		}
		catch(Exception e)
		{
			return new CommandResponse()
			{
				Status = CommandResultStatus.Error,
				Response = e.Message
			};
		}
	}

	private static string PrintChildren(Node node)
	{
		var response = new StringBuilder();

		response.AppendLine($"Children of [{node.Name}]");

		foreach(var c in node.GetChildren())
		{
			response.AppendLine(c.GetPath().ToString().Replace("/root", ""));
		}

		return response.ToString();
	}

	private static string PrintProperties(Node node)
	{
		var properties = node.GetPropertyList();

		var sb = new StringBuilder();

		sb.AppendLine($"Properties of [{node.Name}]");

		foreach(var p in properties.Where(p => p["type"].AsInt32() != (int) Variant.Type.Nil))
		{
			sb.AppendLine($"- {p["name"]}");
		}

		return sb.ToString();
	}
}
