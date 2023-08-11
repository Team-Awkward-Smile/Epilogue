using Epilogue.global.enums;
using Epilogue.global.singletons;

using Godot;

using System.Collections.Generic;
using System.Linq;

namespace Epilogue.ui;
/// <summary>
///		Popup responsible for notifying the player of new achievements unlocked
/// </summary>
public partial class AchievementPopup : Control
{
	private List<AchievementData> _achievementData;
	private Control _popup;

	/// <inheritdoc/>
	public override void _Ready()
	{
		_popup = GetChild(0) as Control;
		_popup.Position = new Vector2(_popup.Position.X, -_popup.Size.Y - 10f);

		// TODO: 118 - This should be in a separate file
		_achievementData = new()
		{
			new AchievementData()
			{
				ID = Achievement.Play100Seconds,
				Name = "Addiction",
				Description = "Play for 100 seconds"
			},
			new AchievementData()
			{
				ID = Achievement.Jump10Times,
				Name = "Froggy",
				Description = "Jump 10 times"
			}
		};

		GetNode<Achievements>("/root/Achievements").AchievementUnlocked += DisplayPopup;
	}

	// TODO: 118 - Allow more than one achievement to be unlocked at once, by querying the remaining notifications and displaying them one at a time
	private async void DisplayPopup(Achievement achievementID)
	{
		var data = _achievementData.Where(d => d.ID == achievementID).First();

		// TODO: 118 - Add icons as well
		_popup.GetNode<Label>("Name").Text = data.Name;
		_popup.GetNode<Label>("Description").Text = data.Description;

		var tween = GetTree().CreateTween();
			
		tween.TweenProperty(_popup, "position", new Vector2(_popup.Position.X, 10), 0.5f);

		await ToSignal(tween, "finished");

		GetTree().CreateTimer(5f).Timeout += () =>
		{
			GetTree().CreateTween().TweenProperty(_popup, "position", new Vector2(_popup.Position.X, -_popup.Size.Y - 10), 1f);
		};
	}
}
