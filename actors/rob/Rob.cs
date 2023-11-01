using Epilogue.nodes;

using Godot;

public partial class Rob : Npc
{
	[Export] private float _shotCooldown = 5f;
	[Export] private float _meleeCooldown = 2f;
	[Export] private float _moveSpeed = 80f;
	[Export] private float _fleeSpeed = 100f;

	private protected override void SetUpVariables()
	{
		CustomVariables.Add("ShotCooldown", _shotCooldown);
		CustomVariables.Add("MeleeCooldown", _meleeCooldown);
		CustomVariables.Add("AttackTimer", 0f);
		CustomVariables.Add("FleeDuration", 0f);
		CustomVariables.Add("CanShoot", true);
		CustomVariables.Add("MoveSpeed", _moveSpeed);
		CustomVariables.Add("FleeSpeed", _fleeSpeed);
	}

	private protected override void ProcessFrame(double delta)
	{
		CustomVariables["AttackTimer"] = CustomVariables["AttackTimer"].AsSingle() + (float) delta;
	}

	private protected override void OnDamageTaken(float damage, float currentHp)
	{
		CustomVariables["CanShoot"] = false;
	}

	private protected override void OnGrowl(float effectStrength)
	{
		CustomVariables["FleeDuration"] = effectStrength / 10f;
		StateMachine.ChangeState("Flee");
	}

	private protected override void OnStunExpired()
	{
		CustomVariables["MoveSpeed"] = 100f;
		GetNode<HitBox>("FlipRoot/HitBox").BonusDamage = 2f;
	}
}
