namespace Epilogue.ui.console;
/// <summary>
///		Status of the Response of the executed command
/// </summary>
public enum CommandResultStatus
{
	/// <summary>
	///		The command was executed successfully
	/// </summary>
	Success,

	/// <summary>
	///		The command was executed, but some minor errors ocurred
	/// </summary>
	Warning,

	/// <summary>
	///		The command could not be executed
	/// </summary>
	Error
}
