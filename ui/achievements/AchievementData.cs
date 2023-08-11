using Epilogue.global.enums;

using Godot;

namespace Epilogue.ui;
/// <summary>
///		Object used to hold the data of achievements to be displayed to be player
/// </summary>
public class AchievementData
{
    public Achievement ID { get; set; }
    public Texture2D Icon { get; set; }
	public string Name { get; set; }
	public string Description { get; set; }
}
