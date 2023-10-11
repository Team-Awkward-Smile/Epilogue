using Epilogue.constants;
using Godot;
using System;

public class JumpData
{
    public Vector2 StartPosition { get; set; } = Vector2.Zero;

    public Vector2 EndPosition { get; set; } = Vector2.Zero;

    public Vector2 Distance => new(Mathf.Abs(EndPosition.X - StartPosition.X), Mathf.Abs(EndPosition.Y - StartPosition.Y));

    public Vector2 MaxSpeed { get; set; } = Vector2.Zero;

    public float Duration { get; set; } = 0f;

    public Vector2 Tiles => new(Mathf.Abs(EndPosition.X - StartPosition.X) / Constants.MAP_TILE_SIZE, Mathf.Abs(EndPosition.Y - StartPosition.Y) / Constants.MAP_TILE_SIZE);
}
