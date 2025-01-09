using Godot;

public partial class TestingMenu : CanvasLayer
{
	const string PATH = "res://gameplay/testing_levels/";

	private Node2D _currentScene;
	private SubViewport _subViewport;
	private Label _title;

	public override void _Ready()
	{
		GetTree().Paused = false;

		_subViewport = GetNode<SubViewport>("%SubViewport");
		_title = GetNode<Label>("%Title");

		var dir = DirAccess.Open(PATH);

		if (dir is not null)
		{
			dir.ListDirBegin();

			var fileName = dir.GetNext();

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

		newBtn.Connect(BaseButton.SignalName.Pressed, Callable.From(() => UpdatePreview(newBtn)));

		GetNode("%ScenesList").AddChild(newBtn);
	}

	public void UpdatePreview(Button btn)
	{
		if (_currentScene is not null)
		{
			_subViewport.RemoveChild(_currentScene);
		}

		_subViewport.RenderTargetClearMode = SubViewport.ClearMode.Always;

		_currentScene = (Node2D)GD.Load<PackedScene>($"{PATH}{btn.Text}/{btn.Text}.tscn").Instantiate();

		_title.Text = btn.Text;
		_subViewport.AddChild(_currentScene);

		_currentScene.ProcessMode = ProcessModeEnum.Disabled;
	}

	public void _on_start_scene_btn_button_down()
	{
		var scene = GD.Load<PackedScene>(PATH + _title.Text + "/" + _title.Text + ".tscn");

		GetTree().ChangeSceneToPacked(scene);
	}
}
