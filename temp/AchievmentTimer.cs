using Epilogue.global.singletons;

using Godot;

public partial class AchievmentTimer : Timer
{
	public override void _Ready()
	{
		var achievement = GetNode<Achievements>("/root/Achievements");

		Timeout += () => achievement.PlayTimeOver100 = true;
	}
}
