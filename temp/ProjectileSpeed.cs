using Godot;

namespace Epilogue.temp;
/// <summary>
///		Class used to measure projectile speeds
/// </summary>
public partial class ProjectileSpeed : Area2D
{
	private float _timer;

	/// <inheritdoc/>
	public override void _Ready()
	{
		AreaEntered += (Area2D area) => GD.Print($"[{GlobalPosition.X}] - {_timer}");
	}

	/// <inheritdoc/>
	public override void _Process(double delta)
	{
		_timer += (float)delta;
	}
}
