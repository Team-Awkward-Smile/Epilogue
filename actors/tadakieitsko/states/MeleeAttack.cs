using Epilogue.Global.States;
using Godot;

namespace Epilogue.Actors.TadakiEiTsko.States
{
	/// <summary>
	/// Handles the melee attack state for Tadaki Ei' Tsko (TET).
	/// </summary>
	public partial class MeleeAttack : NpcState<TadakiEiTsko>
	{
		/// <summary>
		/// Executes when TET enters the melee attack state.
		/// </summary>
		/// <param name="npc">The Tadaki Ei' Tsko instance.</param>
		public override void Enter(TadakiEiTsko npc)
		{
			// Move towards the player with a set attack speed
			npc.Velocity = (npc.Player.GlobalPosition - npc.GlobalPosition).Normalized() * 100f;
		}
	}
}
