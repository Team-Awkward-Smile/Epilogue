using Epilogue.Global.Enums;
using Epilogue.Nodes;
using Godot.Collections;

namespace Epilogue.Actors.TerraBischem;
/// <summary>
///		Version of a Terra Bischem that can swing forwards and backwards in a 2.5D manner
/// </summary>
public partial class SwingingAxe : Npc
{
	/// <inheritdoc/>
	public override Dictionary<DamageType, float> DamageModifiers { get; set; } = new()
	{
		{ DamageType.Fire, 2f },
		{ DamageType.Light, 0.5f }
	};

	private protected override bool UseDefaultPathfinding => false;

	/// <inheritdoc/>
	public override void _Ready()
	{
		base._Ready();

		AnimationPlayer.Play("swing");
	}

	private protected override void OnDamageTaken(float damage, float currentHp, DamageType damageType)
	{
		return;
	}

	private protected override void OnExecutionPerformed(ExecutionSpeed executionSpeed)
	{
		return;
	}

	private protected override void OnGrowl(GrowlType growlType)
	{
		return;
	}

	private protected override void OnHealthDepleted(DamageType damageType)
	{
		QueueFree();
	}

	private protected override void OnPlayerDeath()
	{
		return;
	}

	private protected override void OnVulnerabilityTriggered()
	{
		return;
	}

	private protected override void ProcessFrame(double delta)
	{
		return;
	}
}
