using Epilogue.global.enums;
using Epilogue.global.singletons;

namespace Epilogue.util;
public static class InputUtils
{
	/// <summary>
	///		Appends the suffix "_modern" or "_retro" to the name of the input action, depending on the selected control scheme
	/// </summary>
	/// <param name="action">The input action desired</param>
	/// <returns>The same input action, plus "_modern" or "_retro" appended to it</returns>
	public static string GetInputActionName(string action)
	{
		return $"{action}_{(Settings.ControlScheme == ControlSchemeEnum.Modern ? "modern" : "retro")}";
	}
}
