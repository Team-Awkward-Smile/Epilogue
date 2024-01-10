using Godot;

namespace Epilogue.Nodes;
/// <summary>
///		Base class for every UI Screen
/// </summary>
[GlobalClass]
public partial class UIScreen : Control
{
	/// <summary>
	///		Enables this Screen
	/// </summary>
	/// <param name="pauseTree">Should the Tree be paused before this Screen opens?</param>
	public virtual void Enable(bool pauseTree = false)
	{
		GetTree().Paused = pauseTree;
		Show();
	}

	/// <summary>
	///		Disables (hides) this Screen
	/// </summary>
	/// <param name="unpauseTree">Should the Tree be unpaused before this Screen closes?</param>
	public virtual void Disable(bool unpauseTree = false)
	{
		GetTree().Paused = !unpauseTree;
		Hide();
	}

	/// <summary>
	///		Deletes this Screen from the Tree
	/// </summary>
	/// <param name="unpauseTree">Should the Tree be unpaused before this Screen is deleted?</param>
	public virtual void Close(bool unpauseTree = false)
	{
		GetTree().Paused = !unpauseTree;
		QueueFree();
	}
}
