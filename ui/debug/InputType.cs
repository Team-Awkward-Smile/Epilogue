using Epilogue.Global.Singletons;
using Godot;

namespace Epilogue.UI.debug;
/// <summary>
/// 	Displays the type of the last input received (PC or Controller)
/// </summary>
public partial class InputType : Label
{
	/// <inheritdoc/>
	public override void _Process(double delta)
	{
		Text = InputDeviceManager.MostRecentInputType.ToString();
	}
}
