using Godot;
using Epilogue.nodes;
using Epilogue.util;
using Epilogue.extensions;

namespace Epilogue.actors.tupois.states;
public partial class Walk : StateComponent
{
	private double _walkDuration = 3f;
	private double _timer = 0f;
	private float _walkSpeed = -50f;
	private double _shaderTimer = 0f;

	public override void OnEnter()
	{
		AnimPlayer.Play("tupois/Walk");
	}

	public override void Update(double delta)
	{
		_shaderTimer += delta;

		var pos = ShaderUtils.GetCanvasItemPositionInScreenUV(Actor);

		//Actor.Sprite.SetShaderMaterialParameter("center", pos);
	}
}

