using Godot;

namespace Epilogue.ui.popup;
/// <summary>
///		Object used when creating custom buttons with the <see cref="CustomPopup"/> class
/// </summary>
public class PopupButton
{
	/// <summary>
	///		Text of the button, displayed as a Label
	/// </summary>
	public string ButtonText { get; set; }

	/// <summary>
	///		Should this button be created at the right of other buttons?
	/// </summary>
	public bool Right { get; set; } = true;

	/// <summary>
	///		Action triggered whenever this button is clicked. See <see cref="AcceptDialog.CustomAction"/>
	/// </summary>
	public string Action { get; set; }
}
