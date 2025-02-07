using Epilogue.Actors.TadakiEiTsko.States;
using Epilogue.Global.States;
using Godot;

namespace Epilogue.Actors.TadakiEiTsko.States
{
	/// <summary>
	/// State that allows the Tadaki Ei' Tsko (TET) to remain idle while not engaged in combat.
	/// </summary>
	public partial class Idle : NpcState<TadakiEiTsko>
	{
		private readonly RandomNumberGenerator _rng;
		private readonly float _idleMinTime;
		private readonly float _idleMaxTime;
		private readonly float _detectionRange;

		private float _timer;
		private float _idleTime;

		public Idle(StateMachine stateMachine, float idleMinTime, float idleMaxTime, float detectionRange) : base(stateMachine)
		{
			_rng = new RandomNumberGenerator();
			_idleMinTime = idleMinTime;
			_idleMaxTime = idleMaxTime;
			_detectionRange = detectionRange;
		}

		public override void Enter(TadakiEiTsko npc)
		{
			// Start idle animation
			npc.AnimationPlayer.Play("idle");

			_timer = 0f;
			_idleTime = _rng.RandfRange(_idleMinTime, _idleMaxTime);
		}

		public override void Update(TadakiEiTsko npc, double delta)
		{
			if ((_timer += (float)delta) >= _idleTime)
			{
				// Transition to wandering if idle time has passed
				StateMachine.ChangeState(typeof(Wander));
				return;
			}

			// If player is detected within range, transition to combat
			if (npc.DistanceFromPlayer <= _detectionRange)
			{
				StateMachine.ChangeState(typeof(Combat));
				return;
			}
		}
	}
}
