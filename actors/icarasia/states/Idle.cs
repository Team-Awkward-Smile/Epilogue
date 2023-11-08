using Epilogue.nodes;
using Godot;
using System;

namespace Epilogue.actors.icarasia.states;
public partial class Idle : NpcState
{
    private float _timer = 0f;

    internal override void OnEnter(params object[] args)
    {
        _timer = 0f;
    }

    internal override void PhysicsUpdate(double delta)
    {
        var waveX = Mathf.Sin(_timer += ((float) delta * 3f)) / 2f;
        var waveY = Mathf.Cos(_timer) / 2f;

        Npc.GlobalPosition += new Vector2(waveX, waveY);
    }
}
