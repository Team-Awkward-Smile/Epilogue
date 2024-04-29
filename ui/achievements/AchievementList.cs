using Epilogue.Global.Enums;
using Epilogue.UI;
using System.Collections.Generic;

namespace Epilogue.Achievements;
/// <summary>
///		Helper class used to define the list of available achievements without cluttering the rest of the code
/// </summary>
public class AchievementList
{
	/// <summary>
	///		List of every achievement in the game containing their IDs, names, and description
	/// </summary>
	public List<AchievementData> Achievements = new()
	{
		new AchievementData()
		{
			ID = Achievement.Addiction,
			Name = "Addiction",
			Description = "Play for 100 seconds"
		},
		new AchievementData()
		{
			ID = Achievement.Froggy,
			Name = "Froggy",
			Description = "Jump 10 times"
		},
		new AchievementData()
		{
			ID = Achievement.TasteForBlood,
			Name = "Taste for Blood",
			Description = "Kill your first enemy"
		},
		new AchievementData()
		{
			ID = Achievement.YoyoMaster,
			Name = "Yoyo Master",
			Description = "Kill a Terra Bischem for the first time"
		}
	};
}
