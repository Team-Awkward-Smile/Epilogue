using Godot;

namespace Epilogue.nodes;
/// <summary>
///		Base class for every UI Screen
/// </summary>
[GlobalClass]
public partial class UI : Control
{
	/// <summary>
	///		Enables this Screen
	/// </summary>
	public virtual void Enable()
	{
		Show();
	}

	/// <summary>
	///		Disables this Screen
	/// </summary>
	public virtual void Disable()
	{
		Hide();
	}
}
