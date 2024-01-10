using Godot;
using Godot.Collections;

namespace Epilogue.nodes;
public abstract partial class HitBoxManager : Node
{
    private protected abstract Dictionary<string, Callable> HitBoxAnimations { get; set; }

    public void PlayHitBoxAnimation(string key)
    {
        HitBoxAnimations[key].Call();
    }
}
