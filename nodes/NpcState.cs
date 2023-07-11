using Godot;
using System.Linq;

namespace Epilogue.nodes;
public partial class NpcState : State
{
	protected NavigationAgent2D _navigationAgent;

	public override void _Ready()
	{
		base._Ready();

		_navigationAgent = Actor.GetChildren().OfType<NavigationAgent2D>().FirstOrDefault();
	}
}
