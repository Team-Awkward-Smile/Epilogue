using Epilogue.Actors.TerraBischem;
using Epilogue.Global.Enums;
using Epilogue.Nodes;
using Godot;

namespace Epilogue.Global.Singletons;
/// <summary>
///		Singleton in charge of storing values used by achievements, and unlocking those achievements when their conditions are met
/// </summary>
public partial class AchievementManager : Node
{
	private float _timer = 0f;
	private PlayerEvents _playerEvents;
	private NpcEvents _npcEvents;
	private int _jumpCount;
	private int _killedNpcs;
	private double _playTime;

	/// <summary>
	///		Event triggered whenever an achievement is unlocked
	/// </summary>
	/// <param name="achievement">The ID of the new achievement</param>
	[Signal] public delegate void AchievementUnlockedEventHandler(Achievement achievement);

	/// <inheritdoc/>
	public override void _Ready()
	{
		_playerEvents = GetNode<PlayerEvents>("/root/PlayerEvents");
		_npcEvents = GetNode<NpcEvents>("/root/NpcEvents");

		_playerEvents.PlayerJumped += AchievementJump10Times;
		_npcEvents.EnemyKilled += AchievementKillFirstEnemy;
		_npcEvents.EnemyKilled += AchievementKillFirstTerraBischem;

		GetTree().CreateTimer(100f).Timeout += () => EmitSignal(SignalName.AchievementUnlocked, (int)Achievement.Addiction);
	}

	private void AchievementJump10Times()
	{
		if (++_jumpCount >= 10)
		{
			EmitSignal(SignalName.AchievementUnlocked, (int)Achievement.Froggy);

			_playerEvents.PlayerJumped -= AchievementJump10Times;
		}
	}

	private void AchievementKillFirstEnemy(Npc npc)
	{
		if (++_killedNpcs >= 1)
		{
			EmitSignal(SignalName.AchievementUnlocked, (int)Achievement.TasteForBlood);

			_npcEvents.EnemyKilled -= AchievementKillFirstEnemy;
		}
	}

	private void AchievementKillFirstTerraBischem(Npc npc)
	{
		if (npc is YoyoTerraBischem)
		{
			EmitSignal(SignalName.AchievementUnlocked, (int)Achievement.YoyoMaster);

			_npcEvents.EnemyKilled -= AchievementKillFirstTerraBischem;
		}
	}
}
