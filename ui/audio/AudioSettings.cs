using Epilogue.global.singletons;
using Epilogue.nodes;
using Godot;
using System.Linq;

namespace Epilogue.ui.audio;
/// <summary>
///		Screen responsible for changing the volume of different audio buses
/// </summary>
public partial class AudioSettings : UI
{
	/// <inheritdoc/>
	public override void _Ready()
	{
		var buses = GetNode("MarginContainer/VBoxContainer").GetChildren().OfType<VBoxContainer>();

		foreach(var bus in buses)
		{
			var slider = bus.GetNode<HSlider>("Slider");
			var index = AudioServer.GetBusIndex(bus.Name);
			var volumeLinear = Mathf.Round(Mathf.DbToLinear(AudioServer.GetBusVolumeDb(index)) * 100);

			bus.GetNode<Label>("Value").Text = volumeLinear.ToString();

			slider.Value = volumeLinear;

			slider.DragEnded += (bool valueChanged) =>
			{
				UpdateVolume(slider.Value, bus.Name, bus.GetNode<Label>("Value"));
			};
		}
	}

	private void UpdateVolume(double volume, StringName bus, Label valueLabel)
	{
		valueLabel.Text = volume.ToString();

		var volumeDb = (float) Mathf.LinearToDb(volume / 100f);
		var index = AudioServer.GetBusIndex(bus);

		AudioServer.SetBusVolumeDb(index, volumeDb);

		var audioPlayer = GetNode<AudioStreamPlayer>("AudioStreamPlayer");

		audioPlayer.Bus = bus;
		audioPlayer.Play(0.75f);

		Settings.SaveSettings();
	}
}
