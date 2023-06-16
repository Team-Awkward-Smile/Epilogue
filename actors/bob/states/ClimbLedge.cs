using Epilogue.constants;
using Epilogue.nodes;
using Godot;

namespace Epilogue.actors.hestmor.states;
public partial class ClimbLedge : StateComponent
{
	public override void OnEnter()
	{
		Character.Position = new Vector2(Character.Position.X + Constants.MAP_TILE_SIZE, Character.Position.Y - (Constants.MAP_TILE_SIZE / 2));
		StateMachine.ChangeState("Idle");
	}
}
