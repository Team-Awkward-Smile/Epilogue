using Epilogue.Global.Enums;
using Epilogue.nodes;
using Godot;
using Godot.Collections;

namespace Epilogue.actors.hestmor;
public partial class HestmorHitBoxManager : HitBoxManager
{
    private protected override Dictionary<string, Callable> HitBoxAnimations { get; set; } = new();

    private Player _player;

    public override void _Ready()
    {
        _player = (Player)Owner;

        HitBoxAnimations = new()
        {
            { "melee_attack", Callable.From(MeleeAttack) }
        };
    }

    private void MeleeAttack()
    {
        var hitBox = new HitBox()
        {
            Position = new Vector2(21, -33),
            CollisionShape = GD.Load<Shape2D>("res://actors/bob/hitboxes/melee_1.tres"),
            Damage = 1f,
            DamageType = DamageType.Unarmed,
            LifeTime = 0.2f,
            CollisionLayer = (int)CollisionLayerName.PlayerHitBox,
            CollisionMask = (int)(CollisionLayerName.World | CollisionLayerName.NpcHurtBox)
        };

        hitBox.Connect(HitBox.SignalName.TileHit, Callable.From((bool isTileBreakable) =>
        {
            if (!isTileBreakable)
            {
                _player.ActorAudioPlayer.PlayRandomCollisionSfx("ScratchRock");
            }
        }), (uint)ConnectFlags.OneShot);

        _player.GetNode<Node2D>("FlipRoot").AddChild(hitBox);
    }
}
