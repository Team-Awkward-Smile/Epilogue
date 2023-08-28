using Godot;

namespace Epilogue.ui.debug;
public partial class BranchName : Label
{
	/// <inheritdoc/>
	public override void _Ready()
	{
		Text = "Current Branch:\n" + FileAccess.Open("res://.git/HEAD", FileAccess.ModeFlags.Read).GetAsText().Replace("ref: refs/heads/", "");
	}
}
