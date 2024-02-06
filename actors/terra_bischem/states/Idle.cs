using Epilogue.Nodes;
using Godot;
using System.Threading.Tasks;

namespace Epilogue.Actors.TerraBischem.States;
/// <inheritdoc/>
public partial class Idle : State
{
	private readonly YoyoTerraBischem _terraBischem;
	private readonly RandomNumberGenerator _rng;
	private readonly float _detectionRange;

	private Tween _tween;
	private bool _canChangeState = true;

	/// <summary>
	///		State that allows a Yoyo Terra Bischem to look around while waiting for the player
	/// </summary>
	/// <param name="stateMachine"></param>
	/// <param name="detectionRange"></param>
	public Idle(StateMachine stateMachine, float detectionRange) : base(stateMachine)
	{
		_terraBischem = (YoyoTerraBischem)stateMachine.Owner;
		_rng = new RandomNumberGenerator();
		_detectionRange = detectionRange;
	}

	// args[0] - bool - Defines if the Terra Bischem will engage in combat once the player is close enough
	internal override void OnEnter(params object[] args)
	{
		if (args.Length > 0)
		{
			_canChangeState = (bool)args[0];
		}

		TweenEyeRotation();
	}

	internal override void Update(double delta)
	{
		if (_canChangeState && _terraBischem.DistanceToPlayer <= _detectionRange)
		{
			StateMachine.ChangeState(typeof(Combat));
		}
	}

	internal override Task OnLeave()
	{
		if (_tween.IsValid())
		{
			_tween.Kill();
		}

		return Task.CompletedTask;
	}

	private void TweenEyeRotation()
	{
		var rotation = _rng.RandfRange(0f, 360f);
		var delay = _rng.RandfRange(2f, 4f);

		_tween = StateMachine.CreateTween();

		_tween.TweenProperty(_terraBischem.Sprite, "rotation_degrees", rotation, 1f).SetDelay(delay);

		_tween.Finished += TweenEyeRotation;
	}
}
