using Godot;
using System;

namespace Epilogue.nodes;
[GlobalClass, Icon("res://nodes/icons/comment.png"), Tool]
public partial class Comment : Node2D
{
	[Export] public string Text { get; set; }

	public override void _Ready()
	{
		if(!Engine.IsEditorHint())
        {
            QueueFree();
        }

		if(GetChildCount() == 0)
		{
			AddChild(new TextureRect() 
			{
				Texture = GD.Load<CompressedTexture2D>("res://nodes/icons/comment_hires.png"),
				ExpandMode = TextureRect.ExpandModeEnum.IgnoreSize,
				Size = new(30f, 30f),
				StretchMode = TextureRect.StretchModeEnum.Scale,
				Position = new(-15f, -15f)
			});
		}
	}
}
