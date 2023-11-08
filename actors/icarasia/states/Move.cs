using Epilogue.global.enums;
using Epilogue.nodes;
using Godot;
using System;

namespace Epilogue.actors.icarasia.states;
public partial class Move : NpcState
{
    [Export] private float _moveSpeed = 80f;

    internal override void PhysicsUpdate(double delta)
    {
        Npc.Velocity = (Player.GlobalPosition - Npc.GlobalPosition).Normalized() * _moveSpeed;

        if(Npc.Velocity.X != 0f)
        {
            Npc.SetFacingDirection(Npc.Velocity.X > 0f ? ActorFacingDirection.Right : ActorFacingDirection.Left);
        }

        Npc.MoveAndSlide();
    }
}
