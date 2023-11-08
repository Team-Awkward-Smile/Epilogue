using Epilogue.nodes;
using Godot;
using System;

namespace Epilogue.actors.icarasia.states;
public partial class Shoot : NpcState
{
    private int angle;
    private Node2D _projectilePivot;
    private PackedScene _projectileScene;

    private protected override void AfterReady()
    {
        base.AfterReady();

        _projectilePivot = Npc.GetNode<Node2D>("FlipRoot/ProjectilePivot");
        _projectileScene = GD.Load<PackedScene>("res://actors/icarasia/projectiles/projectile.tscn");
    }

    internal override void OnEnter(params object[] args)
    {
        angle = (int) args[0];

        _projectilePivot.RotationDegrees = angle;

        var projectile = _projectileScene.Instantiate() as Projectile;

        GetTree().GetLevel().AddChild(projectile);

        projectile.GlobalTransform = _projectilePivot.GetNode<Node2D>("ProjectileSpawn").GlobalTransform;

        Npc.CustomVariables["AttackTimer"] = 0f;

        StateMachine.ChangeState("Move");
    }
}
