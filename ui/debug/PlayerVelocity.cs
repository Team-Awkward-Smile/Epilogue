using Godot;

namespace Epilogue.ui.debug;
public partial class PlayerVelocity : Label
{
	private CharacterBody2D _player;

	public override void _Ready()
	{
		_player = (CharacterBody2D) GetTree().GetFirstNodeInGroup("Player");
		Size = new Vector2(130f, 50f);
	}

	public override void _Process(double delta)
	{
		Text = $"Player Speed:\n({_player.Velocity.X:0.0}, {_player.Velocity.Y:0.0})";
	}
}
