using Godot;

[Tool]
public partial class BaseScene : Node2D
{
	public override void _Ready()
	{
		var versionNumber = ProjectSettings.GetSetting("global/Version").AsString();
		var versionLabel = new Label()
		{
			Name = "VersionLabel",
			Text = $"Version [{versionNumber}] / Branch [{VersionManager.GetBranchName()}]"
		};

		var ui = new CanvasLayer();

		ui.AddChild(versionLabel);
		versionLabel.AnchorsPreset = 2;

		AddChild(ui);
	}
}
