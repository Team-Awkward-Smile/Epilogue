using Godot;
using System;

public partial class UILayer : CanvasLayer
{	
	private PackedScene _loading_icon = GD.Load<PackedScene>("ui/scene_loader/loading_icon/LoadingIcon.tscn"); 
 
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_AddLoadingIcon();
	}

	private void _AddLoadingIcon() 
	{
		LoadingIcon _new_loading_icon = _loading_icon.Instantiate<LoadingIcon>();
		AddChild(_new_loading_icon);
	}

}
