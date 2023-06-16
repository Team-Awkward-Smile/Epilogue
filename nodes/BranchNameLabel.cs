using Godot;

namespace Epilogue.nodes
{
	[GlobalClass]
	public partial class BranchNameLabel : CanvasLayer
	{
		public override void _Ready()
		{
			var label = new Label
			{
				Text = FileAccess.Open("res://.git/HEAD", FileAccess.ModeFlags.Read).GetAsText().Replace("ref: refs/heads/", "")
			};

			AddChild(label);
		}
	}
}
