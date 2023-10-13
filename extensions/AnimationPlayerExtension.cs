using Godot;
using System;

namespace Epilogue.extensions;
public static class AnimationPlayerExtension
{
    public static void Reset(this AnimationPlayer animationPlayer)
    {
        animationPlayer.Play("RESET");
        animationPlayer.Advance(0f);
    }
}
