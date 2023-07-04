using Epilogue.global.singletons;
using Godot;

namespace Epilogue.ui.debug;
public partial class InputType : Label
{
	public override void _Process(double delta)
	{
		Text = InputDeviceManager.MostRecentInputType.ToString();
	}
}
