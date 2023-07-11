using Epilogue.constants;
using Epilogue.nodes;
using Godot;

namespace Epilogue.actors.hestmor.states;
public partial class ClimbLedge : PlayerState
{
	public override void OnEnter()
	{
		Actor.Position = new Vector2(Actor.Position.X + Constants.MAP_TILE_SIZE, Actor.Position.Y - (Constants.MAP_TILE_SIZE / 2));
		StateMachine.ChangeState("Idle");
	}
}
