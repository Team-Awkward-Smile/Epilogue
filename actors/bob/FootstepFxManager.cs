using Epilogue.nodes;
using Epilogue.util;
using Godot;
using Godot.Collections;
using System.Linq;

namespace Epilogue.actors.hestmor;
public partial class FootstepFxManager : Node
{
	private AudioPlayerBase _audioPlayer;
	private Level _level;
	private Dictionary<string, Node2D> _feet = new();

	public override void _Ready()
	{
		_audioPlayer = Owner.GetChildren().OfType<AudioPlayer>().First();
		_level = GetTree().Root.GetChildren().OfType<Level>().FirstOrDefault();

		GetChildren().OfType<Node2D>().ToList().ForEach(c =>
		{
			_feet.Add(c.Name, c);
		});
	}

	public void PlayFootstepSfx(string nodeName)
	{
		var foot = _feet[nodeName];
		var pos = new Vector2(foot.GlobalPosition.X, foot.GlobalPosition.Y + 5);
		var groundType = (string) _level.GetTileDataAtPosition(pos)?.GetCustomData("ground_type");

		if(groundType is not null)
		{
			_audioPlayer.PlayFootstep(groundType);
		}
	}
}
