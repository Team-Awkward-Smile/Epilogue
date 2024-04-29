using Epilogue.Achievements;
using Epilogue.Global.Enums;
using Epilogue.Global.Singletons;
using Godot;
using System.Collections.Generic;
using System.Linq;

namespace Epilogue.UI;
/// <summary>
///		Popup responsible for notifying the player of new achievements unlocked
/// </summary>
public partial class AchievementPopup : Control
{
	/// <summary>
	///		Signal emitted whenever the Achievement Popup finishes it's animation and becomes hidden
	/// </summary>
	[Signal] public delegate void PopupHiddenEventHandler();

	private readonly List<Achievement> _queriedAchievements = new();

	private AchievementList _achievementList;
	private Control _popup;
	private Label _nameLabel;
	private Label _descriptionLabel;
	private TextureRect _iconRect;
	private bool _popupVisible = false;

	/// <inheritdoc/>
	public override void _Ready()
	{
		_popup = GetChild(0) as Control;

		_nameLabel = _popup.GetNode<Label>("Name");
		_descriptionLabel = _popup.GetNode<Label>("Description");
		_iconRect = _popup.GetNode<TextureRect>("Icon");

		_popup.Position = new Vector2(_popup.Position.X, -_popup.Size.Y - 10f);

		_achievementList = new AchievementList();

		GetNode<AchievementManager>("/root/AchievementManager").AchievementUnlocked += QueryAchievementPopup;

		PopupHidden += () =>
		{
			if (_queriedAchievements.Any())
			{
				DisplayPopup();
			}
		};
	}

	// Queries a popup to be displayed
	private void QueryAchievementPopup(Achievement achievement)
	{
		_queriedAchievements.Add(achievement);

		if (!_popupVisible)
		{
			DisplayPopup();
		}
	}

	// Displays a popup for the first queried achievement. If there are any remaining achievements, more popups will be displayed until no more achievements remain
	private async void DisplayPopup()
	{
		_popupVisible = true;

		var achievement = _queriedAchievements.First();
		var data = _achievementList.Achievements.Where(d => d.ID == achievement).First();

		_nameLabel.Text = data.Name;
		_descriptionLabel.Text = data.Description;
		_iconRect.Texture = GD.Load<CompressedTexture2D>($"res://ui/achievements/icons/{data.ID}.png");

		var showTween = GetTree().CreateTween();
			
		showTween.TweenProperty(_popup, "position", new Vector2(_popup.Position.X, 10), 0.5f);

		await ToSignal(showTween, "finished");

		GetTree().CreateTimer(5f).Timeout += async () =>
		{
			_queriedAchievements.Remove(achievement);

			var hideTween = GetTree().CreateTween();
			
			hideTween.TweenProperty(_popup, "position", new Vector2(_popup.Position.X, -_popup.Size.Y - 10), 1f);

			await ToSignal(hideTween, "finished");

			_popupVisible = false;

			EmitSignal(SignalName.PopupHidden);
		};
	}
}
