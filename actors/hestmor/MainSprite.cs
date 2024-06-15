using Epilogue.Actors.Hestmor.Enums;
using Epilogue.Global.DTO;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Epilogue.Actors.Hestmor;
public partial class MainSprite : Sprite2D
{
	private readonly List<SpriteSheetDataDto> _spriteSheetData = new()
	{
		new SpriteSheetDataDto()
		{
			ID = (int)SpriteSheetId.IdleWalk,
			Texture = GD.Load<CompressedTexture2D>("res://actors/hestmor/sprite_sheet/hestmor_sheet1.png"),
			HFrames = 8,
			VFrames = 3,
			Scale = new Vector2(1f, 1f)
		},
		new SpriteSheetDataDto()
		{
			ID = (int)SpriteSheetId.Bob,
			Texture = GD.Load<CompressedTexture2D>("res://actors/hestmor/sprite_sheet/epilogue_bob-Sheet.png"),
			HFrames = 11,
			VFrames = 36,
			Scale = new Vector2(1.7f, 1.7f)
		}
	};

	public void UpdateSpriteSheet(SpriteSheetId id)
	{
		var sheet = _spriteSheetData.First(s => s.ID == (int)id);

		Frame = 0;

		Texture = sheet.Texture;
		Hframes = sheet.HFrames;
		Vframes = sheet.VFrames;
		Scale = sheet.Scale;
	}
}

