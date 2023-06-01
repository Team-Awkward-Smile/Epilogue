using Godot;

public partial class VersionManager : Node
{
	public static string GetBranchName()
	{
		using var file = FileAccess.Open("res://.git/HEAD", FileAccess.ModeFlags.Read);
		var text = file.GetAsText();

		return text.Replace("ref: refs/heads/", "").Replace("\n", "");
	}
}
