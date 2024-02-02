using Godot;

namespace Epilogue.Nodes;
/// <summary>
/// 	Node that emits blood particles whenever an Actor takes damage
/// </summary>
[GlobalClass, Tool] public partial class BloodEmitter : GpuParticles2D
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

	/// <inheritdoc/>
	public override void _EnterTree()
	{
		Emitting = false;
		ProcessMaterial = GD.Load<ParticleProcessMaterial>("res://base_resources/blood_particle.tres");

		_particleMaterial = ProcessMaterial as ParticleProcessMaterial;
	}

	/// <inheritdoc/>
	public void EmitBlood(int amount = 50)
	{
		Amount = amount;
		Emitting = true;
	}
}
