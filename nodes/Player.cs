using Epilogue.actors.hestmor;
using Godot;
using System.Linq;

namespace Epilogue.nodes;
/// <summary>
///		Node used exclusively by the Player Character
/// </summary>
[GlobalClass]
public partial class Player : Actor
{
	/// <summary>
	///     Reference to the ToggleRunRetro Node
	/// </summary>
	public MovementInputManager MovementInputManager { get; set; }

	public override void _Ready()
	{
		base._Ready();

		MovementInputManager = (MovementInputManager) GetNode<Node>("MovementInputManager");
	}
}
