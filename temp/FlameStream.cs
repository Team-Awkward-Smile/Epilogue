using Godot;
using Epilogue.nodes;
using System.Collections.Generic;

namespace Epilogue.guns;
/// <summary>
///		Temporary projectile used for testing purposes
/// </summary>
public partial class FlameStream : Line2D
{
	public List<Projectile> Projectiles { get; set; } = new();

	private float _timer = 0f;

	public override void _Ready()
	{
		ClearPoints();
	}

	public void AddProjectile(Projectile projectile)
	{
		Projectiles.Add(projectile);
		AddPoint(ToLocal(projectile.Position));

		projectile.TreeExiting += () =>
		{
			RemovePoint(GetPointCount() - 1);
			Projectiles.Remove(projectile);
		};
	}

	public override void _Process(double delta)
	{
		_timer += (float) delta;

		if(_timer > 0.2f)
		{
			_timer = 0f;
		}

		if(Projectiles.Count == 0)
		{
			QueueFree();
		}

		var i = Projectiles.Count - 1;

		foreach(var projectile in Projectiles)
		{
			SetPointPosition(i--, ToLocal(projectile.Position));
		}
	}
}

