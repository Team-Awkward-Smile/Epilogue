using Epilogue.global.enums;
using Epilogue.nodes;

using Godot;

public partial class Move : NpcState
{
	private bool _isIdle = false;

	internal override void OnEnter()
	{
		Npc.CanChangeFacingDirection = true;
		AnimPlayer.PlayBackwards("walk");
	}

	internal override void PhysicsUpdate(double delta)
	{
		var canShoot = Npc.CustomVariables["CanShoot"].AsBool();
		var timer = Npc.CustomVariables["AttackTimer"].AsSingle();
		var distance = Npc.PlayerNavigationAgent2D.DistanceToTarget();
		var velocity = Vector2.Zero;
		var moveSpeed = Npc.CustomVariables["MoveSpeed"].AsSingle();

		if(!Npc.IsPlayerReachable)
		{
			StateMachine.ChangeState("Wander");

			return;
		}
		else if(distance > (canShoot ? 200f : 30f))
		{
			velocity = Npc.PlayerNavigationAgent2D.GetNextVelocity(Npc.GlobalPosition, moveSpeed);
		}
		else if(canShoot && distance <= 150f)
		{
			velocity = Npc.PlayerNavigationAgent2D.GetNextVelocity(Npc.GlobalPosition, moveSpeed) * -1;
		}
		else
		{
			var angle = Mathf.RadToDeg(Mathf.Atan2(Player.GlobalPosition.Y - Npc.GlobalPosition.Y, Player.GlobalPosition.X - Npc.GlobalPosition.X));

			var shotAimed = Mathf.Abs(angle) switch
			{
				<= 2 or >= 178 => true,
				_ => false
			};

			if(canShoot && shotAimed && timer >= Npc.CustomVariables["ShotCooldown"].AsSingle())
			{
				StateMachine.ChangeState("Shoot");
				Npc.CustomVariables["AttackTimer"] = 0f;

				return;
			}
			else if(!canShoot && timer >= Npc.CustomVariables["MeleeCooldown"].AsSingle())
			{
				StateMachine.ChangeState("Attack");
				Npc.CustomVariables["AttackTimer"] = 0f;

				return;
			}
		}

		Npc.Velocity = new Vector2(velocity.X, velocity.Y + Gravity * (float) delta);
		Npc.MoveAndSlideWithRotation();

		if(Npc.Velocity.X != 0)
		{
			Npc.SetFacingDirection(Npc.Velocity.X > 0 ? ActorFacingDirection.Right : ActorFacingDirection.Left);

			if(_isIdle)
			{
				AnimPlayer.PlayBackwards("walk");
				_isIdle = false;
			}
		}
		else
		{
			Npc.TurnTowards(Player);
			AnimPlayer.PlayBackwards("idle");
			_isIdle = true;
		}
	}
}
