using Epilogue.global.enums;
using Epilogue.global.objects;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Epilogue.addons.character_base
{
	/// <summary>
	///		Base class to be used by all characters in the game (both the player and NPCs)
	/// </summary>
	public partial class CharacterBase : CharacterBody2D
	{
		private CharacterAction _currentAction;

		private readonly List<CharacterAction> _actionList = new()
		{
			new CharacterAction(ActionName.Idle, ActionPriority.Lowest, false),
			new CharacterAction(ActionName.Walking, ActionPriority.Low, false),
			new CharacterAction(ActionName.Crouched, ActionPriority.Medium, true),
			new CharacterAction(ActionName.Attacking, ActionPriority.High, true),
			new CharacterAction(ActionName.Jumping, ActionPriority.High, false),
			new CharacterAction(ActionName.Dead, ActionPriority.Highest, true)
		};

		/// <summary>
		///		The current action being performed by the character
		/// </summary>
		public CharacterAction CurrentAction 
		{
			get => _currentAction;
			set
			{
				var oldAction = _currentAction ?? _actionList.Where(a => a.Action == ActionName.Idle).First();

				_currentAction = value;

				// Emits a signal whenever the current action changes. May be used by other nodes to trigger events
				EmitSignal(SignalName.CurrentActionChanged, (int) _currentAction.Action, (int) oldAction.Action);
			}
		}

		[Signal] public delegate void CurrentActionChangedEventHandler(ActionName newAction, ActionName oldAction);

		public override void _Ready()
		{
			CurrentAction = _actionList.Where(a => a.Action == ActionName.Idle).First();
		}

		/// <summary>
		///		Tries to change the character's current action. If the new action has a higher priority than the current one, the change will occur
		/// </summary>
		/// <param name="action">The new action attempted to be performed by the character</param>
		/// <returns><c>true</c>, if the current action was successfully changed; otherwise, <c>false</c></returns>
		public bool TrySetAction(ActionName action)
		{
			var newAction = _actionList.Where(a => a.Action == action).First();

			if(newAction.Priority >= CurrentAction.Priority && newAction.Action != CurrentAction.Action)
			{
				CurrentAction = newAction;

				return true;
			}

			return false;
		}

		/// <summary>
		///		Works just like <see cref="TrySetAction(ActionName)"/>, but the action is set using it's name.
		///		Useful to set actions during animation frames
		/// </summary>
		/// <param name="actionName">The name of the action to try to set</param>
		public bool TrySetActionString(string actionName)
		{
			if(Enum.TryParse(actionName, out ActionName action))
			{
				return TrySetAction(action);
			}

			return false;
		}

		/// <summary>
		///		Tries to clear the character's current action. If the current action is the same as the one informed, the character will return to Idle. 
		///		The parameter works as a security feature, to prevent cases where a Node removes an action that does not belong to it
		/// </summary>
		/// <param name="action">The current action of the character. If this action is not the same as the current one, nothing will happen. Otherwise, the character will return to idle</param>
		public void ClearAction(ActionName action)
		{
			if(CurrentAction.Action == action)
			{
				CurrentAction = _actionList.Where(a => a.Action == ActionName.Idle).First();
			}
		}

		/// <summary>
		///		Works just like <see cref="ClearAction(ActionName)"/>, but the action is cleared using it's name.
		///		Useful to clear actions during animation frames
		/// </summary>
		/// <param name="actionName">The name of the action to try to remove</param>
		public void ClearActionString(string actionName)
		{
			if(Enum.TryParse(actionName, out ActionName action) && CurrentAction.Action == action)
			{
				CurrentAction = _actionList.Where(a => a.Action == ActionName.Idle).First();
			}
		}
	}
}
