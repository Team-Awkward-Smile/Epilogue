using Epilogue.UI.Pause;
using Godot;
using Godot.NativeInterop;
using Microsoft.VisualBasic;
using System;
using System.IO;
using System.Linq;

public partial class TestingMenu : CanvasLayer
{
	const string PATH = "res://gameplay/testing_levels/";
	private Node2D _currentScene;
	private Godot.SubViewport _subViewport;
	private Godot.Label _Title;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_subViewport = GetNode<Godot.SubViewport>("%SubViewport");
		_Title = GetNode<Godot.Label>("%Title");

		var dir = DirAccess.Open(PATH);
		if (dir != null)
		{
			dir.ListDirBegin();
			string fileName = dir.GetNext();
			while (fileName != "")
			{
				if (dir.CurrentIsDir())
				{
					GD.Print($"Found directory: {fileName}");
					AddToList(fileName);
				}
				else
				{
					GD.Print($"Found file: {fileName}");
				}
				fileName = dir.GetNext();
			}
		}
	}

	private void AddToList(string fileName)
	{
		var newBtn = new Button
		{
			Text = fileName
		};
		Action myAction = () => { UpdatePreview(newBtn); };
		newBtn.Connect(Button.SignalName.ButtonDown, Callable.From(myAction));
		GetNode("%ScenesList").AddChild(newBtn);
	}

	public void UpdatePreview(Button Btn)
	{
		_subViewport.RenderTargetClearMode = Godot.SubViewport.ClearMode.Always;

		_currentScene = (Node2D)GD.Load<PackedScene>(PATH+Btn.Text+"/"+Btn.Text+".tscn").Instantiate();
		_Title.Text = Btn.Text;
		_subViewport.AddChild(_currentScene);
		
		GetNode<Timer>("Timer").Start();
	}

	public void _on_timer_timeout()
	{
		_currentScene.QueueFree();
		_subViewport.RenderTargetClearMode = Godot.SubViewport.ClearMode.Never;
	}

	public void _on_start_scene_btn_button_down()
	{
		_currentScene = (Node2D)GD.Load<PackedScene>(PATH+_Title.Text+"/"+_Title.Text+".tscn").Instantiate();
		GetParent().AddChild(_currentScene);
		Hide();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
