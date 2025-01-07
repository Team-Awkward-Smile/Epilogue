using Godot;
using System;

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

		Action myAction = () => UpdatePreview(newBtn);
		newBtn.Connect(Button.SignalName.ButtonDown, Callable.From(myAction));

		GetNode("%ScenesList").AddChild(newBtn);
	}

	public void UpdatePreview(Button Btn)
	{
		if (_currentScene is not null)
		{
			_subViewport.RemoveChild(_currentScene);
		}

		_subViewport.RenderTargetClearMode = SubViewport.ClearMode.Always;

		_currentScene = (Node2D)GD.Load<PackedScene>($"{PATH}{Btn.Text}/{Btn.Text}.tscn").Instantiate();

		_Title.Text = Btn.Text;
		_subViewport.AddChild(_currentScene);

		_currentScene.ProcessMode = ProcessModeEnum.Disabled;
	}

	public void _on_start_scene_btn_button_down()
	{
		var scene = GD.Load<PackedScene>(PATH + _Title.Text + "/" + _Title.Text + ".tscn");

		GetTree().ChangeSceneToPacked(scene);
	}
}
