namespace Epilogue.global.enums;
/// <summary>
///		Type of the RemapButton used to determine the event read by said button
/// </summary>
public enum RemapButtonType
{
	/// <summary>
	///		Primary PC button (read the first PC event)
	/// </summary>
	PcPrimary,

	/// <summary>
	///		Secondary PC button (read the second PC event, if any)
	/// </summary>
	PcSecondary,

	/// <summary>
	///		Controller button (read the joypad event)
	/// </summary>
	Controller
}
