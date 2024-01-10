using Epilogue.Global.Enums;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Epilogue.Global.Singletons;
/// <summary>
///		Singleton in charge of storing values used by achievements, and unlocking those achievements when their conditions are met
/// </summary>
public partial class Achievements : Node
{
	/// <summary>
	/// 	Number of times the player has jumped during the course of the game
	/// </summary>
    public int JumpCount { get; set; }

	/// <summary>
	/// 	Defines if the player has played the game for at least 100 seconds
	/// </summary>
    public bool PlayTimeOver100 { get; set; }

    private Dictionary<Achievement, Func<bool>> _unlockCriteria;
	private float _timer = 0f;

	/// <summary>
	///		Event triggered whenever an achievement is unlocked
	/// </summary>
	/// <param name="achievement">The ID of the new achievement</param>
	[Signal] public delegate void AchievementUnlockedEventHandler(Achievement achievement);

	/// <inheritdoc/>
	public override void _Ready()
	{
		_unlockCriteria = new()
		{
			{ Achievement.Jump10Times, () => JumpCount >= 10 },
			{ Achievement.Play100Seconds, () => PlayTimeOver100 }
		};
	}

	/// <inheritdoc/>
	public override void _PhysicsProcess(double delta)
	{
		if((_timer += (float) delta) < 1f)
		{
			return;
		}

		_timer = 0f;

		foreach(var achievment in _unlockCriteria.ToList())
		{
			if(achievment.Value.Invoke())
			{
				// TODO: 118 - Set the Achievement as unlocked in the Settings singleton from branch 148
				_unlockCriteria.Remove(achievment.Key);

				EmitSignal(SignalName.AchievementUnlocked, (int) achievment.Key);
			}
		}
	}
}
