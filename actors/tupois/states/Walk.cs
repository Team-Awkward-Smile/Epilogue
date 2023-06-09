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

		Actor.GetNode<Sprite2D>("Sprite2D").SetShaderMaterialParameter("center", pos);
	}

	// Called once per logic frame (60 times per second, by default)
	public override void PhysicsUpdate(double delta)
	{
		_timer += delta;

		if(_timer > _walkDuration)
		{
			_timer = 0f;
			_walkSpeed *= -1;
		}

		Actor.Velocity = new Vector2(_walkSpeed * (float) delta * 100f, 0f);
		Actor.MoveAndSlide();
	}
}

