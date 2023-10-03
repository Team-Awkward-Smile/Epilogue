using Epilogue.global.singletons;
using Epilogue.nodes;
using Godot;
using static Godot.DisplayServer;

namespace Epilogue.ui.video;
/// <summary>
///		Screen responsible for changing different video settings
/// </summary>
public partial class VideoSettings : UI
{
	/// <inheritdoc/>
	public override void _Ready()
	{
		GetNode<OptionButton>("%WindowOption").ItemSelected += (long index) =>
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
