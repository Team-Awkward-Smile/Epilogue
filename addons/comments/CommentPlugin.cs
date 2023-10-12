#if TOOLS
using Epilogue.nodes;
using Godot;
using System;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

[Tool]
public partial class CommentPlugin : EditorPlugin
{
	private Comment _comment;
	private Label _text;

	public override bool _Handles(GodotObject @object)
	{
		return @object is Comment;
	}

	public override void _Edit(GodotObject @object)
	{
		if(@object is null)
		{
			_comment?.GetChildren().OfType<Label>().FirstOrDefault()?.QueueFree();
		}

		if(@object is Comment cmt)
		{
			_comment = cmt;
		}
	}

	public override bool _ForwardCanvasGuiInput(InputEvent @event)
	{
		if(@event.IsReleased() && @event is InputEventMouseButton btn && btn.ButtonIndex == MouseButton.Left)
		{
			_text = new()
			{
				AnchorsPreset = 8,
				Position = new(-15f, -50f),
				VerticalAlignment = VerticalAlignment.Center,
				HorizontalAlignment = HorizontalAlignment.Center,
				LabelSettings = GD.Load<LabelSettings>("res://addons/comments/label_settings.tres"),
				Size = new(200f, 0f),
				AutowrapMode = TextServer.AutowrapMode.Word,
				Text = _comment.Text,
				ZIndex = 100
			};

			_comment.AddChild(_text);
		}

		return base._ForwardCanvasGuiInput(@event);
	}
}
#endif
