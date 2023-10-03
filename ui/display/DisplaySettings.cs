using Epilogue.global.enums;
using Epilogue.global.singletons;
using Epilogue.nodes;
using Godot;

namespace Epilogue.ui.display;
/// <summary>
///		Screen responsible for settings that change what's displayed on-screen
/// </summary>
public partial class DisplaySettings : UI
{
	/// <inheritdoc/>
	public override void _Ready()
	{
		var brandOption = GetNode<OptionButton>("%BrandOption");

		brandOption.Select((int) Settings.ControllerType);
		brandOption.ItemSelected += (long index) =>
		{
			Settings.ControllerType = (InputDeviceBrand) index;
			Settings.SaveSettings();
		};
	}
}
