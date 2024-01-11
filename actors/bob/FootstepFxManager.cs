using Epilogue.Nodes;
using Godot;
using Godot.Collections;
using System.Linq;

namespace Epilogue.actors.hestmor;
/// <summary>
///		Node responsible for playing footstep SFX
/// </summary>
public partial class FootstepFxManager : Node
{
	private ActorAudioPlayer _audioPlayer;
	private Level _level;
	private Dictionary<string, Node2D> _feet = new();

	/// <inheritdoc/>
	public override void _Ready()
	{
		_audioPlayer = Owner.GetChildren().OfType<AudioPlayer>().First();
		_level = GetTree().Root.GetChildren().OfType<Level>().FirstOrDefault();

		GetChildren().OfType<Node2D>().ToList().ForEach(c =>
		{
			_feet.Add(c.Name, c);
		});
	}

	/// <summary>
	///		Plays a footstep SFX according to the informed Node's position. The resulting audio depends on the corresponding tile and the SFX collection implemented in <see cref="ActorAudioPlayer.CollisionSfxList"/>
	/// </summary>
	/// <param name="nodeName"></param>
	public void PlayFootstepSfx(string nodeName)
	{
		var foot = _feet[nodeName];
		var pos = new Vector2(foot.GlobalPosition.X, foot.GlobalPosition.Y + 5);
		var groundType = (string) _level.GetTileDataAtPosition(pos)?.GetCustomData("ground_type");

		if(groundType is not null)
		{
			_audioPlayer.PlayCollisionSfx(groundType);
		}
	}
}
