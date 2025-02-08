using Epilogue.Global.Enums;
using Godot;

namespace Epilogue.Actors.Hestmor.aim;
/// <summary>
///		Node responsible for handling aiming inputs from a mouse
/// </summary>
public partial class MouseAim : Node
{
	private Aim _aim;
	private Viewport _viewport;
	private Node2D _aimPivot;

	/// <inheritdoc/>
	public override void _Ready()
	{
		_aim = (Aim)GetParent();
		_viewport = GetViewport();
		_aimPivot = Owner.GetNode<Node2D>("GunSystem/AimPivot");
	}

	/// <inheritdoc/>
	public override void _Process(double delta)
	{
		var pivotScreenPosition = _aimPivot.GetGlobalTransformWithCanvas().Origin;
		var mousePosition = (_viewport.GetMousePosition() - pivotScreenPosition) * new Vector2(1f, -1f);
		var angle = Mathf.RadToDeg(Mathf.Atan2(mousePosition.Y, mousePosition.X)) + 22.5f;
		var wheelArea = Mathf.Floor(angle / 45f);

		var flagX = AimDirection.None;
		var flagY = angle >= 0 ? AimDirection.Up : AimDirection.Down;

		switch (Mathf.Abs(wheelArea))
		{
			case 0:
				flagY = AimDirection.None;
				flagX = AimDirection.Right;
				break;

			case 1:
				flagX = AimDirection.Right;
				break;

			case 2:
				flagX = AimDirection.None;
				break;

			case 3:
				flagX = AimDirection.Left;
				break;

			case 4:
				flagY = AimDirection.None;
				flagX = AimDirection.Left;
				break;
		}

		_aim.SetAimDirection(flagX | flagY);
	}
}