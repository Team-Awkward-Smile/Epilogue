using Epilogue.global.enums;
using Godot;

namespace Epilogue.actors.hestmor
{
	public partial class AttackController : Node2D
	{
		private AnimationNodeStateMachinePlayback _stateMachine;

		public override void _Ready()
		{
			_stateMachine = (AnimationNodeStateMachinePlayback) GetNode<AnimationTree>("%AnimationTree").Get("parameters/playback");
		}
	}
}
