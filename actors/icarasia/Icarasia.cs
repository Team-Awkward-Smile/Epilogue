using Epilogue.nodes;
using Godot;
using System;

namespace Epilogue.actors.icarasia;
public partial class Icarasia : Npc
{
    [Export] private float _detectionDistance = 200f;
    [Export] private float _shotCooldown = 5f;

    private float _distanceToPlayer;
    private float _projectileSweepTimer = 0f;
    private bool _isIsCombatMode = false;

    private protected override void SetUpVariables()
    {
        CustomVariables.Add("ShotCooldown", _shotCooldown);
        CustomVariables.Add("AttackTimer", 0f);
    }

    private protected override void ProcessFrame(double delta)
    {
        CustomVariables["AttackTimer"] = CustomVariables["AttackTimer"].AsSingle() + (float) delta;

        _distanceToPlayer = Player.GlobalPosition.DistanceTo(GlobalPosition);

        if(_distanceToPlayer <= _detectionDistance)
        {
            StateMachine.ChangeState("Move");
            _isIsCombatMode = true;
        }

        if(_isIsCombatMode && (_projectileSweepTimer += (float) delta) >= 0.1f)
        {
            _projectileSweepTimer = 0f;

            var angle = SweepProjectileRayCastForPlayer();

            if(angle is not null && CustomVariables["AttackTimer"].AsSingle() >= _shotCooldown)
            {
                StateMachine.ChangeState("Shoot", angle);
            }
        }
    }

    private int? SweepProjectileRayCastForPlayer()
    {
        var raycast = RayCasts["Projectile"];
        var angles = new[] { 0, 45, 135, 180, 225, 315 };

        raycast.Enabled = true;

        foreach(var angle in angles)
        {
            raycast.RotationDegrees = angle;
            raycast.ForceRaycastUpdate();

            if(raycast.IsColliding() && (raycast.GetCollider() is Player))
            {
                raycast.Enabled = false;

                return angle;
            }
        }

        raycast.Enabled = false;

        return null;
    }
}
