using Godot;
using Godot.Collections;
using System.Linq;

namespace Epilogue.Nodes;
/// <summary>
///     An AudioStreamPlayer that has a list of predefined streams and plays one of them at random
/// </summary>
public partial class MultiAudioStreamPlayer : AudioStreamPlayer
{
    [Export] private Array<AudioStream> _audioStreams;

    private RandomNumberGenerator _rng = new();

    /// <summary>
    ///     Plays a random stream from the predefined array
    /// </summary>
    public void PlayRandom()
    {
        var sfx = _audioStreams.ElementAt(_rng.RandiRange(0, _audioStreams.Count - 1));

        Stream = sfx;

        Play();
    }
}
