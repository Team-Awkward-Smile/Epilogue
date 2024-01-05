using Epilogue.Global.Enums;

namespace Epilogue.Global.DTO;
/// <summary>
///		Class with data about a Growl that's about to be performed
/// </summary>
public class GrowlDTO
{
	/// <summary>
	///		Animation to be used for this Growl
	/// </summary>
	public string Animation { get; set; }

	/// <summary>
	///		Radius (in units) this Growl will affect
	/// </summary>
	public float Radius { get; set; }

	/// <summary>
	///		Type of this Growl
	/// </summary>
	public GrowlType GrowlType { get; set; }
}
