using Epilogue.global.singletons;

using Godot;

namespace Epilogue.ui;
public partial class RemapButton : Button
{
	private InputEvent _inputEvent;
	private PopupPanel _popup;
	private InputDeviceManager _deviceManager;

    public string ActionName { get; set; }
    public InputEvent InputEvent
	{
		get => _inputEvent;
		set
		{
			_inputEvent = value;
			Icon = _deviceManager.GetKeyIcon(value);
		}
	}

	public override void _Ready()
	{
		_popup = GetNode<PopupPanel>("../../../../PopupPanel");
		_deviceManager = GetNode<InputDeviceManager>("/root/InputDeviceManager");

		ButtonDown += () =>
		{
			_popup.GetNode<Label>("Label").Text = ActionName + "\n\nPress any button";
			_popup.Hide();
			_popup.PopupCentered();
			_popup.WindowInput += ListenToInput;
			_popup.PopupHide += HidePopup;
			_popup.Exclusive = false;
		};

		Size = new Vector2(350f, 30f);
		TextOverrunBehavior = TextServer.OverrunBehavior.TrimEllipsis;
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
