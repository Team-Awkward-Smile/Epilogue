using Epilogue.Nodes;
using Godot;

namespace Epilogue.Guns.Flamethrower;
/// <summary>
///		Base projectile Node used by the Flamethrower
/// </summary>
public partial class Flame : Projectile
{
	/// <inheritdoc/>
	public override void _PhysicsProcess(double delta)
	{
		base._PhysicsProcess(delta);

		Speed = new Vector2(Speed.X, Speed.Y - (20 * (float)delta));
	}
}
