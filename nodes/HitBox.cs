using Godot;

namespace Epilogue.nodes;
[GlobalClass]
public partial class HitBox : Area2D
{
	/// <summary>
	///		Damage caused by this HitBox
	/// </summary>
	[Export] public float Damage { get; private set; }
}
