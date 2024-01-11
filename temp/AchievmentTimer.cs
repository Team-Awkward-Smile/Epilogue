using Epilogue.Global.Singletons;
using Godot;

namespace Epilogue.temp;
/// <summary>
/// 	Temporary Node that controls for how long the player has been playing. Unlocks an anchievement once the playtime reaches 100 seconds
/// </summary>
public partial class AchievmentTimer : Timer
{
	/// <inheritdoc/>
	public override void _Ready()
	{
		var achievement = GetNode<Achievements>("/root/Achievements");

		Timeout += () => achievement.PlayTimeOver100 = true;
	}
}
