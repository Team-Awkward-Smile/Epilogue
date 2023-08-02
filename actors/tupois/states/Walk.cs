using Epilogue.nodes;

namespace Epilogue.actors.tupois.states;
/// <summary>
///		State that allows the Tupois to move around the map
/// </summary>
public partial class Walk : NpcState
{
	private double _walkDuration = 3f;
	private double _timer = 0f;
	private float _walkSpeed = -50f;
	private double _shaderTimer = 0f;

	internal override void OnEnter()
	{
		AnimPlayer.Play("tupois/Walk");
	}

	internal override void Update(double delta)
	{
	}
}

