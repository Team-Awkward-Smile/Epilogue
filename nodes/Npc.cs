using Epilogue.Const;
using Epilogue.Extensions;
using Epilogue.Global.Enums;
using Epilogue.Global.Singletons;
using Godot;
using Godot.Collections;
using System.Linq;
using System.Threading.Tasks;

namespace Epilogue.Nodes;
/// <summary>
///     Base Node for every NPC in the game
/// </summary>
[Icon("res://nodes/icons/npc.png")]
public abstract partial class Npc : Actor
{
	/// <summary>
	///     When the NPC's Current HP is equal to or below this value, it will become Vulnerable
	/// </summary>
	[Export] public float VulnerabilityThreshold { get; set; }

	/// <summary>
	///		Time (in seconds) this NPC will remain Vulnerable before recovering.
	///		Has no effect if <see cref="VulnerabilityThreshold"/> = 0
	/// </summary>
	[Export] public float VulnerabilityDuration { get; set; }

	private protected abstract bool UseDefaultPathfinding { get; }

	/// <summary>
	///		Dictionary of damage types and their modifiers
	/// </summary>
	public abstract Dictionary<DamageType, float> DamageModifiers { get; set; }

	/// <summary>
	///		Current Growl type affecting this NPC. If no Growl is active, it will be null
	/// </summary>
	public GrowlType? CurrentGrowlInEffect { get; private protected set; }

	/// <summary>
	///     Defines if this NPC is Vulnerable and able to be Executed
	/// </summary>
	public bool IsVulnerable { get; private set; }

	/// <summary>
	///     NavigationAgent2D used to control this NPC's pathfinding towards the Player
	/// </summary>
	public NavigationAgent2D PlayerNavigationAgent2D { get; set; }

	/// <summary>
	///     NavigationAgent2D used to control this NPC's pathfinding when roaming around the map randomly
	/// </summary>
	public NavigationAgent2D WanderNavigationAgent2D { get; set; }

	/// <summary>
	///     Reference to the Player character
	/// </summary>
	private protected Player Player { get; set; }

	/// <summary>
	///		Reference to the Blood Emitter of this NPC
	/// </summary>
	public BloodEmitter BloodEmitter { get; private protected set; }

	/// <summary>
	///     Defines if the current position of the Player can be reached by this NPC. Used to avoid having to query the NavigationServer every time
	/// </summary>
	public bool IsPlayerReachable { get; set; }

	/// <summary>
	///     Defines if this NPC is waiting for the NavigationServer to update, pausing it's path-finding in the process
	/// </summary>
	public bool WaitingForNavigationQuery { get; set; }

	/// <summary>
	///		Determines if this NPC can recover from the Vulnerable State.
	///		While set to <c>false</c>, the timer to recover will not be updated
	/// </summary>
	public bool CanRecoverFromVulnerability { get; set; } = true;

	private protected NpcStateMachine _npcStateMachine;
	private protected PlayerEvents _playerEvents;
	private protected NpcEvents _npcEvents;

	/// <inheritdoc/>
	public override async void _Ready()
	{
		base._Ready();

		Player = GetTree().GetLevel().Player;

		if (UseDefaultPathfinding)
		{
			await ToSignal(GetTree(), SceneTree.SignalName.PhysicsFrame);

			var navigationAgents = GetChildren().OfType<NavigationAgent2D>();

			PlayerNavigationAgent2D = navigationAgents.First(na => na.Name.ToString().Contains("Player"));
			WanderNavigationAgent2D = navigationAgents.First(na => na.Name.ToString().Contains("Wander"));

			PlayerNavigationAgent2D.TargetPosition = Player.GlobalPosition;

			await ToSignal(GetTree(), "physics_frame");

			IsPlayerReachable = PlayerNavigationAgent2D.IsTargetReachable();
		}

		BloodEmitter ??= GetChildren().OfType<BloodEmitter>().FirstOrDefault();

		_playerEvents = GetNode<PlayerEvents>("/root/PlayerEvents");
		_npcEvents = GetNode<NpcEvents>("/root/NpcEvents");

		_playerEvents.Connect(PlayerEvents.SignalName.PlayerIsDying, Callable.From(OnPlayerDeath));

		_npcStateMachine = GetChildren().OfType<NpcStateMachine>().FirstOrDefault();
		_npcStateMachine?.Activate();
	}

	/// <summary>
	///     Deals damage to this NPC. If its HP then becomes lower than its <see cref="VulnerabilityThreshold"/>, it becomes Vulnerable
	/// </summary>
	/// <param name="damage">Ammount of damage to cause</param>
	/// <param name="damageType">Type of the damage dealt</param>
	public override void ReduceHealth(float damage, DamageType damageType)
	{
		var modifiedDamage = damage + (DamageModifiers.TryGetValue(damageType, out float modifier) ? modifier : 0f);

		if (CurrentHealth == 0 || modifiedDamage <= 0)
		{
			return;
		}

		GD.Print($"Dealing {modifiedDamage} ({damage} + {modifier}) to {Name}");

		BloodEmitter?.EmitBlood();

		CurrentHealth -= modifiedDamage;

		if (CurrentHealth <= 0f)
		{
			if (damageType == DamageType.Unarmed)
			{
				IsVulnerable = true;
			}

			OnHealthDepleted(damageType);

			_npcEvents.EmitSignal(NpcEvents.SignalName.EnemyKilled, this);
		}
		else
		{
			if (!IsVulnerable && CurrentHealth <= VulnerabilityThreshold && VulnerabilityThreshold != 0f)
			{
				IsVulnerable = true;

				OnVulnerabilityTriggered();
			}
			else
			{
				OnDamageTaken(modifiedDamage, CurrentHealth, damageType);
			}
		}
	}

	/// <summary>
	///     Executes this NPC, running the logic that defines what happens
	/// </summary>
	/// <param name="executionSpeed">Speed of the Execution performed (Slow or Fast)</param>
	public void Execute(ExecutionSpeed executionSpeed)
	{
		CurrentHealth = 0;

		OnExecutionPerformed(executionSpeed);
	}

	/// <summary>
	///     Heals this NPC. If it was Vulnerable and it's HP goes above it's <see cref="VulnerabilityThreshold"/>, it will return to normal
	/// </summary>
	/// <param name="health">Ammount of HP to heal</param>
	public override void RecoverHealth(float health)
	{
		CurrentHealth += health;

		if (IsVulnerable && (CurrentHealth > VulnerabilityThreshold))
		{
			IsVulnerable = false;
		}
	}

	/// <summary>
	///     Makes this NPC react to a Growl, taking into account its resistance to the effect
	/// </summary>
	/// <param name="growlType">Base Strength of the effect to be applied. This value may be changed depending on the NPC's resistance to the Growl effect</param>
	public void ReactToGrowl(GrowlType growlType)
	{
		if (CurrentGrowlInEffect is not null)
		{
			return;
		}

		CurrentGrowlInEffect = growlType;

		OnGrowl(growlType);
	}

	/// <inheritdoc/>
	public override async void _PhysicsProcess(double delta)
	{
		// Queries a new path to the Player if the Player moved too far away from the last position
		if (UseDefaultPathfinding && !WaitingForNavigationQuery && Player.GlobalPosition.DistanceTo(PlayerNavigationAgent2D.TargetPosition) > Const.Constants.PATH_REQUERY_THRESHOLD_DISTANCE)
		{
			WaitingForNavigationQuery = true;

			var rng = new RandomNumberGenerator();

			// Awaits between 1 and 10 physics frames before requesting a new path, to avoid having too many NPCs making requests at once
			for (var i = 0; i < rng.RandfRange(0, 10); i++)
			{
				await ToSignal(GetTree(), SceneTree.SignalName.PhysicsFrame);
			}

			PlayerNavigationAgent2D.TargetPosition = Player.GlobalPosition;

			await ToSignal(GetTree(), SceneTree.SignalName.PhysicsFrame);

			IsPlayerReachable = PlayerNavigationAgent2D.IsTargetReachable();

			WaitingForNavigationQuery = false;

			return;
		}

		ProcessFrame(delta);
	}

	/// <summary>
	///     Method used by each NPC that needs to run logic every frame, regardless of it's current State
	/// </summary>
	/// <param name="delta"></param>
	private protected abstract void ProcessFrame(double delta);

	/// <summary>
	///     Method that runs whenever this NPC becomes Vulnerable for the first time
	/// </summary>
	private protected abstract void OnVulnerabilityTriggered();

	/// <summary>
	///     Method that runs whenever this NPC's HP drops to 0
	/// </summary>
	private protected abstract void OnHealthDepleted(DamageType damageType);

	/// <summary>
	///     Method that runs after this NPC takes damage
	/// </summary>
	/// <param name="damage">Damage taken by the NPC</param>
	/// <param name="currentHp">Current HP of this NPC, after damage was applied</param>
	/// <param name="damageType">Type of the damage received</param>
	private protected abstract void OnDamageTaken(float damage, float currentHp, DamageType damageType);

	/// <summary>
	///     Method that runs whenever this NPC is executed
	/// </summary>
	/// <param name="executionSpeed"></param>
	private protected abstract void OnExecutionPerformed(ExecutionSpeed executionSpeed);

	/// <summary>
	///     Method that runs whenever this NPC is affected by Hestmor's Growl
	/// </summary>
	/// <param name="growlType">Type of the Growl that will affect this NPC</param>
	private protected abstract void OnGrowl(GrowlType growlType);

	/// <summary>
	///     Method that runs whenever the Player dies
	/// </summary>
	private protected abstract void OnPlayerDeath();

	/// <summary>
	///     Updates the Wander NavigationAgent2D with the informed point. 
	///     This update takes a random amount of time (between 0 and 10 frames) to make sure that the Navigation Server is not accessed by more than 1 Node at the same time
	/// </summary>
	/// <param name="position">The position that will be assigned to the WanderNavigationAgent2D after 0 ~ 10 frames</param>
	public async Task UpdatePathToWander(Vector2 position)
	{
		var rng = new RandomNumberGenerator();

		// Awaits between 1 and 10 physics frames before requesting a new path, to avoid having too many NPCs making requests at once
		for (var i = 0; i < rng.RandfRange(0, 10); i++)
		{
			await ToSignal(GetTree(), SceneTree.SignalName.PhysicsFrame);
		}

		WanderNavigationAgent2D.TargetPosition = position;
	}
}
