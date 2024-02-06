using Epilogue.Nodes;
using Godot;
using System.Collections.Generic;

namespace Epilogue.UI.Gallery;
/// <summary>
///		Gallery Screen, showing all the available artwork that can be viewed by the player
/// </summary>
public partial class Gallery : Screen
{
	private List<(CompressedTexture2D, string)> _pictureList;
	private CanvasLayer _pictureView;
	private GridContainer _pictureGrid;
	private TextureRect _picture;
	private RichTextLabel _label;
	private int _currentIndex;

	/// <inheritdoc/>
	public override void _UnhandledInput(InputEvent @event)
	{
		if (@event.IsActionPressed("pause_game"))
		{
			if (_pictureView.Visible)
			{
				_pictureView.Hide();
			}
			else
			{
				Hide();
			}
		}

		if (@event is InputEventMouseButton mouseEvent)
		{
			if (_pictureView.Visible && mouseEvent.ButtonIndex is MouseButton.WheelUp or MouseButton.WheelDown)
			{
				SlideCurrentPicture(mouseEvent.ButtonIndex);
			}
			else if (mouseEvent.ButtonIndex is MouseButton.Left or MouseButton.Right && _pictureView.Visible)
			{
				_pictureView.Hide();
			}
		}
	}

	/// <inheritdoc/>
	public override void _Ready()
	{
		_pictureList = new()
		{
			{ (GD.Load<CompressedTexture2D>("res://ui/gallery/pictures/icarasia_alternate.png"), "Icarasia alternate version:\nMade By Alex Mandrei") },
			{ (GD.Load<CompressedTexture2D>("res://ui/gallery/pictures/original_hestmor.png"), "The very first Hestmor doodle:\nMade By Pseudo64") }
		};

		_pictureView = GetNode<CanvasLayer>("PictureView");
		_pictureGrid = GetNode<GridContainer>("%PictureGrid");
		_picture = GetNode<TextureRect>("%Picture");
		_label = GetNode<RichTextLabel>("%LegendText");

		_pictureView.Hide();

		foreach ((CompressedTexture2D texture, string label) in _pictureList)
		{
			var button = new Button()
			{
				Icon = texture,
				CustomMinimumSize = new(192, 108),
				ExpandIcon = true,
				IconAlignment = HorizontalAlignment.Center
			};

			button.ButtonUp += () => DisplayPicture(texture, label);

			_pictureGrid.AddChild(button);
		}
	}

	/// <summary>
	///		Displays a picture to the player
	/// </summary>
	/// <param name="picture">The picture to be displayed</param>
	/// <param name="label">The text to be shown under the picture</param>
	private void DisplayPicture(CompressedTexture2D picture, string label)
	{
		_currentIndex = _pictureList.IndexOf((picture, label));

		_picture.Texture = picture;
		_label.Text = $"[center]{label}[/center]";

		_pictureView.Show();
	}

	/// <summary>
	///		"Slides" the current picture shown left or right, displaying the previous/next available picture respectively (if any).
	///		Sliding left on the first picture or sliding right on the last picture will do nothing
	/// </summary>
	/// <param name="direction">The direction of the slide</param>
	private void SlideCurrentPicture(MouseButton direction)
	{
		var newIndex = Mathf.Clamp(_currentIndex + (direction == MouseButton.WheelUp ? 1 : -1), 0, _pictureList.Count - 1);
		var newPicture = _pictureList[newIndex];

		DisplayPicture(newPicture.Item1, newPicture.Item2);
	}
}
