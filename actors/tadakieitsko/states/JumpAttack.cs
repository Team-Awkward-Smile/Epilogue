using Epilogue.Global.States;
using Godot;

namespace Epilogue.Actors.TadakiEiTsko.States
{
	/// <summary>
	/// Handles the jump attack state for Tadaki Ei' Tsko (TET).
	/// </summary>
	public partial class JumpAttack : NpcState<TadakiEiTsko>
	{
		/// <summary>
		/// Executes when TET enters the jump attack state.
		/// </summary>
		/// <param name="npc">The Tadaki Ei' Tsko instance.</param>
		public override void Enter(TadakiEiTsko npc)
		{
			// Propel upwards to simulate a jump
			npc.Velocity = new Vector2(0, -150);

			// Throw a projectile mid-air
			npc.ThrowProjectile();
		}
	}
}
