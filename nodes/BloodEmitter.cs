using Godot;
using System;

[GlobalClass, Tool]
public partial class BloodEmitter : GpuParticles2D
{
	[Export] private Color BloodColor
	{
		get => _bloodColor;
		set
		{
			_bloodColor = value;

			if(_particleMaterial is not null)
			{
				_particleMaterial.Color = BloodColor;
			}
		}
	}

	private Color _bloodColor;
	private ParticleProcessMaterial _particleMaterial;

	public override void _EnterTree()
	{
		Emitting = false;
		ProcessMaterial = GD.Load<ParticleProcessMaterial>("res://base_resources/blood_particle.tres");

		_particleMaterial = ProcessMaterial as ParticleProcessMaterial;
	}

	public void EmitBlood()
	{
		Emitting = true;
	}
}
