using Epilogue.Global.Enums;

using Godot;

namespace Epilogue.ui;
/// <summary>
///		Object used to hold the data of achievements to be displayed to be player
/// </summary>
public class AchievementData
{
    /// <summary>
    ///     The unique ID of the Achievement
    /// </summary>
    public Achievement ID { get; set; }

    /// <summary>
    ///     The icon of the Achievement displayed to the player
    /// </summary>
    public Texture2D Icon { get; set; }

    /// <summary>
    ///     The name of the Achievement
    /// </summary>
	public string Name { get; set; }

    /// <summary>
    ///     The description of the Achievement
    /// </summary>
	public string Description { get; set; }
}
