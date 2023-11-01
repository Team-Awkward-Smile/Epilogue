namespace Epilogue.constants;
/// <summary>
///		Constants that can be used anywhere
/// </summary>
public static class Constants
{
	/// <summary>
	///		Size of the edge of the map tiles, in pixels
	/// </summary>
	public const int MAP_TILE_SIZE = 18;

	/// <summary>
	///		Minimum distance the Player needs to be from the previous destination to query a new path for Navigation
	/// </summary>
	public const float PATH_REQUERY_THRESHOLD_DISTANCE = 20f;

	/// <summary>
	///		Path for the scene containing the Custom Popup created for this project
	/// </summary>
	public const string CUSTOM_POPUP_SCENE = "res://ui/popup/custom_popup.tscn";
}
