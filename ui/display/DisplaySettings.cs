using Epilogue.Global.Enums;
using Epilogue.Global.Singletons;
using Epilogue.Nodes;
using Godot;

namespace Epilogue.UI.Display;
/// <summary>
///		Screen responsible for settings that change what's displayed on-screen
/// </summary>
public partial class DisplaySettings : Screen
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
