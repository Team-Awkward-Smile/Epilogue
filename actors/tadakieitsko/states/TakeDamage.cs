using Epilogue.Global.States;
using Godot;

namespace Epilogue.Actors.TadakiEiTsko.States
{
	/// <summary>
	/// Handles the state when Tadaki Ei' Tsko (TET) takes damage.
	/// </summary>
	public partial class TakeDamage : NpcState<TadakiEiTsko>
	{
		/// <summary>
		/// Executes when TET enters the take damage state.
		/// </summary>
		/// <param name="npc">The Tadaki Ei' Tsko instance.</param>
		/// <param name="args">Additional arguments (e.g., damage type).</param>
		public override void Enter(TadakiEiTsko npc, params object[] args)
		{
			// Handle damage reaction logic here (e.g., play animation, apply knockback, etc.)
		}
	}
}
