using Epilogue.constants;
using Epilogue.nodes;
using Godot;

namespace Epilogue.actors.hestmor.states;
public partial class GrabLedge : StateComponent
{
	public override void OnInput(InputEvent @event)
	{
		if(Input.IsActionJustPressed("jump"))
		{
			StateMachine.ChangeState("ClimbLedge");
		}
		else if(Input.IsActionJustPressed("crouch"))
		{
			StateMachine.ChangeState("Fall");
		}
	}

	public override void OnEnter()
	{
		Character.Position = new Vector2(Character.Position.X, Character.Position.Y - (Constants.MAP_TILE_SIZE / 2));
	}
}
