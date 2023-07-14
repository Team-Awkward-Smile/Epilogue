using Godot;
using System;
using System.Collections.Generic;

namespace Epilogue.addons;
[GlobalClass, Tool]
public partial class NavigationTrigger : Node2D
{
	private Vector2 _startPoint;
	private Vector2 _endPoint;
	private bool _bidirectional = false;
	private uint _navigationLayer;
	private Color _defaultColor = new(1f, 0.61f, 0.1f);
	private NavigationLink2D _navigationLink = new();

    public bool Selected { get; set; }
    public TileMap TileMap { get; set; }

    [Export] public Vector2 StartPoint 
	{ 
		get => _startPoint;
		set
		{
			_startPoint = value;
			_navigationLink.StartPosition = value;
			QueueRedraw();
		}
	}

    [Export] public Vector2 EndPoint 
	{ 
		get => _endPoint;
		set
		{ 
			_endPoint = value;
			_navigationLink.EndPosition = value;
			QueueRedraw();
		}
	}

    [Export] private bool Bidirectional 
	{ 
		get => _bidirectional;
		set
		{
			_bidirectional = value;
			_navigationLink.Bidirectional = value;
			QueueRedraw();
		}
	}

	[Export(PropertyHint.Layers2DNavigation)] private uint NavigationLayer 
	{ 
		get => _navigationLayer; 
		set
		{
			_navigationLayer = value;
			_navigationLink.NavigationLayers = value;
			UpdateConfigurationWarnings();
			QueueRedraw();
		}
	}

	private int _linkRadius;
	private Font _defaultFont;

	public override string[] _GetConfigurationWarnings()
	{
		var warnings = new List<string>();

		if(NavigationLayer == 0)
		{
			warnings.Add("There are no Navigation Layers selected for this trigger. Actors will be unable to use it");
		}

		return warnings.ToArray();
	}

	public override void _Ready()
	{
		_linkRadius = ProjectSettings.GetSetting("navigation/2d/default_link_connection_radius").AsInt32();
		_defaultFont = GD.Load<Font>("res://base_resources/fonts/rainyhearts.ttf");

		TileMap = GetTree().GetFirstNodeInGroup("TileMap") as TileMap;

		AddChild(_navigationLink);

		_navigationLink.Bidirectional = Bidirectional;

		NavigationLayer = 1;
	}

	public override void _Draw()
	{
		if(!Engine.IsEditorHint() && !OS.IsDebugBuild())
		{
			return;
		}

		DrawCircle(StartPoint, 1.8f, new Color(1, 1, 1));
		DrawCircle(StartPoint, 1.5f, new Color(0.44f, 0.65f, 0.92f));

		DrawCircle(EndPoint, 1.8f, new Color(1, 1, 1));
		DrawCircle(EndPoint, 1.5f, new Color(0.44f, 0.65f, 0.92f));

		var str = "NONE";
		var textColor = new Color(1f, 0f, 0f);
		var layers = new List<string>();
		var maxCharCount = 0;

		for(var i = 0; i < 32; i++)
		{
			if((_navigationLayer & (1 << i)) != 0)
			{
				var layerName = ProjectSettings.GetSetting($"layer_names/2d_navigation/layer_{i + 1}").AsString();

				maxCharCount = Math.Max(maxCharCount, layerName.Length);
				layers.Add(layerName);
			}
		}

		if(layers.Count > 0)
		{
			str = string.Join('\n', layers);
			textColor = new Color(1f, 1f, 1f);
		}
		else
		{
			maxCharCount = 4;
		}

		var position = (StartPoint + EndPoint) / 2 - new Vector2(maxCharCount / 2 * 8f, 0f);

		DrawMultilineStringOutline(_defaultFont, position, str, HorizontalAlignment.Center, -1, 16, 5, 4, new Color(0f, 0f, 0f));
		DrawMultilineString(_defaultFont, position, str, HorizontalAlignment.Center, -1, 16, 5, textColor);

		if(Selected)
		{
			var label1 = Bidirectional ? "Point 1" : "Start";
			var label2 = Bidirectional ? "Point 2" : "End";
			var offset1 = new Vector2(-((label1.Length - 1) / 2) * 8, -5f);
			var offset2 = new Vector2(-((label2.Length - 1) / 2) * 8, -5f);

			DrawStringOutline(_defaultFont, StartPoint + offset1, label1, HorizontalAlignment.Center, -1, 16, 4, new Color(0f, 0f, 0f));
			DrawString(_defaultFont, StartPoint + offset1, label1, HorizontalAlignment.Center, -1, 16);

			DrawStringOutline(_defaultFont, EndPoint + offset2, label2, HorizontalAlignment.Center, -1, 16, 4, new Color(0f, 0f, 0f));
			DrawString(_defaultFont, EndPoint + offset2, label2, HorizontalAlignment.Center, -1, 16);
		}
	}
}
