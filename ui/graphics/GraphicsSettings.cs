using Epilogue.Global.Singletons;
using Epilogue.nodes;
using Godot;
using static Godot.DisplayServer;

namespace Epilogue.ui.video;
/// <summary>
///		Screen responsible for changing different video settings
/// </summary>
public partial class GraphicsSettings : UI
{
	/// <inheritdoc/>
	public override void _Ready()
	{
		var windowOption = GetNode<OptionButton>("%WindowOption");

		GD.Print("--> " + Settings.WindowMode);
		windowOption.Select(Settings.WindowMode switch 
		{
			WindowMode.ExclusiveFullscreen => 0,
			WindowMode.Windowed => 1,
			_ => 2
		});

		windowOption.ItemSelected += (long index) =>
		{
			var mode = index switch
			{
				0 => WindowMode.ExclusiveFullscreen,
				1 => WindowMode.Windowed,
				_ => WindowMode.Fullscreen,
			};

			WindowSetMode(mode);

			Settings.WindowMode = mode;
			Settings.SaveSettings();
		};
	}
}
