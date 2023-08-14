using Godot;

namespace Epilogue.ui;
public partial class RemapButton : Button
{
	private InputEvent _inputEvent;
	private PopupPanel _popup;

    public string ActionName { get; set; }
    public InputEvent InputEvent
	{
		get => _inputEvent;
		set
		{
			_inputEvent = value;
			Text = _inputEvent.AsText().Replace("(Physical)", "");
		}
	}

	public override void _Ready()
	{
		_popup = GetNode<PopupPanel>("../../../../PopupPanel");

		ButtonDown += () =>
		{
			_popup.GetNode<Label>("Label").Text = ActionName + "\n\nPress any button";
			_popup.Hide();
			_popup.PopupCentered();
			_popup.WindowInput += ListenToInput;
			_popup.PopupHide += HidePopup;
			_popup.Exclusive = false;
		};
	}

	private void HidePopup()
	{
		_popup.PopupHide -= HidePopup;
		_popup.WindowInput -= ListenToInput;
	}

	private void ListenToInput(InputEvent @event)
	{
		if(@event is InputEventMouseMotion || @event.IsReleased())
		{
			return;
		}

		_popup.Hide();

		if(InputEvent is null)
		{
			InputMap.ActionAddEvent(ActionName, @event);
		}
		else
		{
			InputMap.ActionEraseEvent(ActionName, InputEvent);
			InputMap.ActionAddEvent(ActionName, @event);
		}

		InputEvent = @event;
	}
}
