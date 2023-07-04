using Epilogue.nodes;
using Godot;
using System.Linq;

namespace Epilogue.actors.hestmor;
public partial class FootstepFxManager : Node
{
	private AudioPlayerBase _audioPlayer;

	public override void _Ready()
	{
		_audioPlayer = Owner.GetChildren().OfType<AudioPlayer>().First();
	}

	public void PlayFootstepSfx(string sfxName)
	{
		var t = ((Actor) Owner).GetLastSlideCollision();
		_audioPlayer.PlayFootstep(sfxName);
	}
}
