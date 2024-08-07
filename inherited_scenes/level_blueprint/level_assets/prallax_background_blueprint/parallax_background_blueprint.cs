using Epilogue.Actors.Icarasia.States;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

[Tool]
public partial class parallax_background_blueprint : ParallaxBackground
{

	/// <summary>
	/// This export velue adds the ability to modify the enum directly from the editor
	/// </summary>
	string bg;
	[Export(PropertyHint.Enum, "None,default")]
	string typeOfBg {
		get {return bg;}
		set {bg = value; ChangeBackground();}
	}

	// Made a univeral path for all backgrounds, we could add more or make big changes
	private static readonly string _PATH = "inherited_scenes/level_blueprint/level_assets/prallax_background_blueprint/assets";

	/// <summary>
	/// You would store the path of all the texture (BackGround, MiddleGround and ForGround)
	/// </summary>
	private Dictionary<string, CompressedTexture2D[]> _background_textures = new()
	{
		{"None" , new CompressedTexture2D[] {null, null, null}},
		{"default" , new CompressedTexture2D[] {
			(CompressedTexture2D)GD.Load($"{_PATH}/default/back_ground.png"),
			(CompressedTexture2D)GD.Load($"{_PATH}/default/middle_ground.png"),
			(CompressedTexture2D)GD.Load($"{_PATH}/default/for_ground.png")}}
	};

	/// <summary>
	/// This function is called whenever the slide down in the editor is changed
	/// </summary>
	private void ChangeBackground() 
	{	
		
		int index = 0;
		foreach (var _paralex_layer in GetChildren())
		{
			Sprite2D sprite = (Sprite2D)_paralex_layer.GetChild(0);
			sprite.Texture = _background_textures.GetValueOrDefault(bg)[index];
			index++; 
		} 
	}
}
