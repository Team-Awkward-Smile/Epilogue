namespace Epilogue.ui.console;
/// <summary>
///		Response object created when a command is executed
/// </summary>
public class CommandResponse
{
    /// <summary>
    ///     Status of the Response
    /// </summary>
    public CommandResultStatus Status { get; set; }

    /// <summary>
    ///     Text returned by the command to be printed on the Console
    /// </summary>
    public string Response { get; set; }
}
