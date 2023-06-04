using Epilogue.addons.character_base;
using Epilogue.global.enums;
using Godot;

namespace Epilogue.actors.hestmor
{
	public partial class AttackController : Node2D
	{
		private CharacterBase _parent;
		private AnimationNodeStateMachinePlayback _stateMachine;

		public override void _Ready()
		{
			_parent = GetParent().GetParent() as CharacterBase;
			_stateMachine = (AnimationNodeStateMachinePlayback) GetNode<AnimationTree>("%AnimationTree").Get("parameters/playback");
		}

		public override void _Input(InputEvent @event)
		{
			if(Input.IsActionJustPressed("attack") && _parent.TrySetAction(ActionName.Attacking))
			{
				_stateMachine.Travel("Attacking");
			}
		}
	}
}
