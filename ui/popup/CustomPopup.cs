using Epilogue.constants;

using Godot;

using System.Collections.Generic;

namespace Epilogue.ui.popup;
public partial class CustomPopup : AcceptDialog
{
	public List<PopupButton> CustomButtons { get; set; } = new();

	public static CustomPopup NewCustomPopup()
	{
		var scene = GD.Load<PackedScene>(Constants.CUSTOM_POPUP_SCENE).Instantiate() as AcceptDialog;

		return scene.Duplicate() as CustomPopup;
	}

	public override void _Ready()
	{
		GetLabel().HorizontalAlignment = HorizontalAlignment.Center;

		foreach(var button in CustomButtons)
		{
			AddButton(button.ButtonText, button.Right, button.Action);
		}
	}
}
