using Godot;
using System.Collections.Generic;

namespace Epilogue.UI.popup;
/// <summary>
///		Custom Popup with the default texture used in this project, and methods to easily add more buttons
/// </summary>
public partial class CustomPopup : AcceptDialog
{
	/// <summary>
	///		List of new buttons that will be automatically added to the Popup
	/// </summary>
	public List<PopupButton> CustomButtons { get; set; } = new();

	/// <summary>
	///		Creates a new instance of the Popup, copying every visual aspect from the corresponding scene
	/// </summary>
	public static CustomPopup NewCustomPopup()
	{
		var scene = GD.Load<PackedScene>(Const.Constants.CUSTOM_POPUP_SCENE).Instantiate() as AcceptDialog;

		return scene.Duplicate() as CustomPopup;
	}

	/// <inheritdoc/>
	public override void _Ready()
	{
		GetLabel().HorizontalAlignment = HorizontalAlignment.Center;

		foreach (var button in CustomButtons)
		{
			AddButton(button.ButtonText, button.Right, button.Action);
		}
	}
}
