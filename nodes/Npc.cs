using Epilogue.constants;
using Epilogue.extensions;
using Epilogue.global.enums;
using Epilogue.global.singletons;

using Godot;
using Godot.Collections;

using System.Linq;
using System.Threading.Tasks;

namespace Epilogue.nodes;
/// <summary>
///     Base Node for every NPC in the game
/// </summary>
[GlobalClass, Icon("res://nodes/icons/npc.png")]
public abstract partial class Npc : Actor
{
    /// <summary>
    ///     When the NPC's Current HP is equal to or below this value, it will become Vulnerable
    /// </summary>
    [Export] public float VulnerabilityThreshold { get; set; }

    [Export] private float _vulnerabilityTimer;

    [Export] private bool _useDefaultPathfinding = true;

    /// <summary>
    ///     Defines if this NPC is Vulnerable and able to be Executed
    /// </summary>
    public bool IsVulnerable { get; private set; } = false;

    /// <summary>
    ///     Defines if this NPC is Stunned and cannot move
    /// </summary>
    public bool IsStunned
    {
        get => _isStunned;
        set
        {
            _isStunned = value;

            if(value)
            {
                OnStunTriggered();
            }
            else
            {
                OnStunExpired();
            }
        }
    }

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

    private protected BloodEmitter BloodEmitter { get; set; }

    private PlayerEvents PlayerEvents { get; set; }

    /// <summary>
    ///     Defines if the current position of the Player can be reached by this NPC. Used to avoid having to query the NavigationServer every time
    /// </summary>
    public bool IsPlayerReachable { get; set; }

    /// <summary>
    ///     Resistance of this NPC to the effects of Hestmor's Growling (0 = 0%; 1 = 100%). Negative values increase the effect
    /// </summary>
    public float GrowlEffectResistance { get; set; } = 0f;

    /// <summary>
    ///     Custom variables implemented by each NPC to be used during gameplay
    /// </summary>
    public Dictionary<string, Variant> CustomVariables { get; set; } = new();

    /// <summary>
    ///     Defines if this NPC is waiting for the NavigationServer to update, pausing it's path-finding in the process
    /// </summary>
    public bool WaitingForNavigationQuery { get; set; }

    private float _vulnerabilityElapsedTime = 0f;
    private bool _isStunned = false;

	public override void _EnterTree()
	{
		CustomVariables["StunTimer"] = _vulnerabilityTimer;
        SetUpVariables();
	}

	/// <inheritdoc/>
	public override async void _Ready()
	{
        base._Ready();

		Player = GetTree().GetLevel().Player;

        if(_useDefaultPathfinding)
        {
		    await ToSignal(GetTree(), "physics_frame");

		    var navigationAgents = GetChildren().OfType<NavigationAgent2D>();

		    PlayerNavigationAgent2D = navigationAgents.Where(na => na.Name.ToString().Contains("Player")).First();
		    WanderNavigationAgent2D = navigationAgents.Where(na => na.Name.ToString().Contains("Wander")).First();
		    PlayerNavigationAgent2D.TargetPosition = Player.GlobalPosition;

		    await ToSignal(GetTree(), "physics_frame");

		    IsPlayerReachable = PlayerNavigationAgent2D.IsTargetReachable();
        }

        BloodEmitter = GetChildren().OfType<BloodEmitter>().FirstOrDefault();
        PlayerEvents = GetNode<PlayerEvents>("/root/PlayerEvents");

        PlayerEvents.Connect(PlayerEvents.SignalName.PlayerDied, Callable.From(OnPlayerDeath));
        StateMachine.Activate();
	}

	/// <summary>
	///     Deals damage to this NPC. If it's HP then becomes lower than it's <see cref="VulnerabilityThreshold"/>, it becomes Vulnerable
	/// </summary>
	/// <param name="damage">Ammount of damage to cause</param>
	public override void DealDamage(float damage)
	{
        BloodEmitter.EmitBlood();

        // Prevent cases where multiple sources dealing damage at once may cause a race-condition
        if(CurrentHealth == 0)
        {
            return;
        }

        CurrentHealth -= damage;

        if(CurrentHealth <= 0f)
        {
            OnHealthDepleted();
        }
        else
        {
            OnDamageTaken(damage, CurrentHealth);

			if(!IsVulnerable && CurrentHealth <= VulnerabilityThreshold)
			{
				IsVulnerable = true;
				
				OnVulnerabilityTriggered();
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
	public override void ApplyHealth(float health)
	{
        CurrentHealth += health;

        if(IsVulnerable && (CurrentHealth > VulnerabilityThreshold))
        {
            IsVulnerable = false;
        }
	}

    /// <summary>
    ///     Makes this NPC react to a Growl, taking into account its resistance to the effect
    /// </summary>
    /// <param name="effectStrength">Base Strength of the effect to be applied. This value may be changed depending on the NPC's resistance to the Growl effect</param>
    public void ReactToGrowl(float effectStrength)
    {
        OnGrowl(effectStrength * (1 - GrowlEffectResistance));
    }

	/// <inheritdoc/>
	public override async void _PhysicsProcess(double delta)
	{
		// Queries a new path to the Player if the Player moved too far away from the last position
		if(_useDefaultPathfinding && !WaitingForNavigationQuery && Player.GlobalPosition.DistanceTo(PlayerNavigationAgent2D.TargetPosition) > Constants.PATH_REQUERY_THRESHOLD_DISTANCE)
		{
			WaitingForNavigationQuery = true;

			var rng = new RandomNumberGenerator();

			// Awaits between 1 and 10 physics frames before requesting a new path, to avoid having too many NPCs making requests at once
			for(var i = 0; i < rng.RandfRange(0, 10); i++)
			{
				await ToSignal(GetTree(), "physics_frame");
			}

			PlayerNavigationAgent2D.TargetPosition = Player.GlobalPosition;

			await ToSignal(GetTree(), "physics_frame");

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
    private protected virtual void ProcessFrame(double delta) { }

    /// <summary>
    ///     Method that runs whenever this NPC becomes Vulnerable for the first time
    ///     Default: Change to State "Stun"
    /// </summary>
    private protected virtual void OnVulnerabilityTriggered()
    {
        IsVulnerable = true;
		Sprite.SetShaderMaterialParameter("iframeActive", true);
        StateMachine.ChangeState("Stun");
    }

    /// <summary>
    ///     Method that runs whenever this NPC's HP drops to 0
    ///     Default: Change to State "Die"
    /// </summary>
    private protected virtual void OnHealthDepleted()
    {
		StateMachine.ChangeState("Die");
    }

    /// <summary>
    ///     Method that runs whenever this NPC takes damage
    ///     Default: do nothing
    /// </summary>
    /// <param name="damage">Damage taken by the NPC</param>
    /// <param name="currentHp">Current HP of this NPC, after damage was applied</param>
    private protected virtual void OnDamageTaken(float damage, float currentHp) { }

    /// <summary>
    ///     Method that runs whenever this NPC is executed
    ///     Default: Change to State "Executed" on a Slow Execution, or "Die" on a Fast Execution
    /// </summary>
    /// <param name="executionSpeed"></param>
    private protected virtual void OnExecutionPerformed(ExecutionSpeed executionSpeed)
    {
        if(executionSpeed == ExecutionSpeed.Slow)
        {
            StateMachine.ChangeState("Executed");
        }
        else
        {
            StateMachine.ChangeState("Die");
        }
    }

    /// <summary>
    ///     Method that runs whenever this NPC is affected by Hestmor's Growl
    ///     Default: do nothing
    /// </summary>
    /// <param name="effectStrength">Strength of the Growl applied to this NPC</param>
    private protected virtual void OnGrowl(float effectStrength) { }

    /// <summary>
    ///     Method used to set up anything needed by each NPC when it's Ready
    /// </summary>
    private protected virtual void SetUpVariables() { }

    /// <summary>
    ///     Method that runs whenever this NPC gets Stunned
    ///     Default: change to State "Stun"
    /// </summary>
    private protected virtual void OnStunTriggered()
    {
        StateMachine.ChangeState("Stun");
    }

    /// <summary>
    ///     Method that runs whenever the Stunned condition affecting this NPC ends
    ///     Default: do nothing
    /// </summary>
    private protected virtual void OnStunExpired() { }

    /// <summary>
    ///     Method that runs whenever the Player dies
    ///     Default: change to State "Wander"
    /// </summary>
    private protected virtual void OnPlayerDeath()
    {
        IsPlayerReachable = false;
        StateMachine.ChangeState("Wander");
    }

    public async Task UpdatePathToWander(Vector2 position)
    {
        var rng = new RandomNumberGenerator();

        // Awaits between 1 and 10 physics frames before requesting a new path, to avoid having too many NPCs making requests at once
        for(var i = 0; i < rng.RandfRange(0, 10); i++)
        {
            await ToSignal(GetTree(), "physics_frame");
        }

        WanderNavigationAgent2D.TargetPosition = position;
    }
}
