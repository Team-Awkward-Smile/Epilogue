
using Epilogue.global.enums;

namespace Epilogue.global.objects
{
	public class CharacterAction
	{
        /// <summary>
        ///     Name of the action
        /// </summary>
        public ActionName Action { get; set; }

        /// <summary>
        ///  Priority of the action
        /// </summary>
        public ActionPriority Priority { get; set; }

        /// <summary>
        ///  Whether this action blocks movement while active or not
        /// </summary>
        public bool BlocksMovement { get; set; }

        public CharacterAction(ActionName action, ActionPriority priority, bool blocksMovement)
        {
            Action = action;
            Priority = priority;
            BlocksMovement = blocksMovement;
        }
    }
}
