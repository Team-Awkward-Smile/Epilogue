using Epilogue.global.enums;
using Epilogue.nodes;

using Godot;

public partial class Rob : Npc
{
	[Export] private float _movementSpeed = 50f;
	[Export] private float _shootCooldown = 5f;
	[Export] private float _meleeCooldown = 5f;

	private Player _player;
	private float _timer = 0f;
	private bool _canShoot = true;
	private RayCast2D _rayCast2D;

	public override void _EnterTree()
	{
		base._EnterTree();

		AfterReady += () =>
		{
			_player = GetTree().GetLevel().Player;

			NavigationAgent2D.TargetDesiredDistance = 20f;

			_rayCast2D = new RayCast2D()
			{
				TargetPosition = _player.Position - GlobalPosition,
				CollisionMask = (int) CollisionLayerName.World
			};

			AddChild(_rayCast2D);
		};
	}

	private protected override void ProcessAI(double delta)
	{
		_timer += (float) delta;
		_rayCast2D.TargetPosition = _player.GlobalPosition - new Vector2(0f, _player.SpriteSize.Y / 2) - GlobalPosition;

		NavigationAgent2D.TargetPosition = _player.Position;

		if(NavigationAgent2D.DistanceToTarget() > 200f)
		{
			var velocity = (NavigationAgent2D.GetNextPathPosition() - GlobalPosition).Normalized() * _movementSpeed;

			Velocity = velocity;
		}
		else
		{
			if(_canShoot && _timer >= _shootCooldown)
			{
				StateMachine.ChangeState("Shoot");
				_timer = 0f;
			}
			else if(_timer >= _meleeCooldown)
			{
				StateMachine.ChangeState("Attack");
				_timer = 0f;
			}
		}
	}

	private protected override void OnDamageTaken(float damage, float currentHp)
	{
		_canShoot = false;

		NavigationAgent2D.TargetDesiredDistance = 50f;
		NavigationAgent2D.PathDesiredDistance = 50f;
	}

	private protected override void OnHealthDepleted()
	{
		StateMachine.ChangeState("Die");
	}

	private protected override void OnGrowl(float effectStrength)
	{
		GD.Print("Strength: " + effectStrength);
	}
}
