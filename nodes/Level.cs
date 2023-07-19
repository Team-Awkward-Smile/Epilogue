using Epilogue.global.singletons;
using Epilogue.ui;
using Godot;
using System.Linq;

namespace Epilogue.nodes;
[GlobalClass]
public partial class Level : Node2D
{
	private PauseUI _pauseUI;
	private Window _console;
	private GloryKillPrompt _killPrompt;

    public override void _Input(InputEvent @event)
	{
		if(@event.IsAction("pause_game") && @event.IsPressed())
		{
			_pauseUI.Show();
			GetTree().Paused =  true;

			GetViewport().SetInputAsHandled();
		}
		else if(@event.IsAction("console") && @event.IsPressed())
		{
			_console.Visible = !_console.Visible;
		}
	}

	public override void _Ready()
	{
		_pauseUI = GD.Load<PackedScene>("res://ui/pause_ui.tscn").Instantiate() as PauseUI;
		_console = GD.Load<PackedScene>("res://ui/console.tscn").Instantiate() as Window;
		_killPrompt = GD.Load<PackedScene>("res://ui/glory_kill_prompt.tscn").Instantiate() as GloryKillPrompt;

		// TODO: 68 - Maybe the root CanvasLayer should also be created at run-time?
		var uiLayer = GetNode<CanvasLayer>("UILayer");

		uiLayer.AddChild(_pauseUI);
		uiLayer.AddChild(_console);
		uiLayer.AddChild(_killPrompt);

		_pauseUI.Hide();
		_console.Hide();
		_killPrompt.Disable();

		_console.Size = DisplayServer.WindowGetSize() / 3;

		ProcessMode = ProcessModeEnum.Pausable;
		GetNode<Events>("/root/Events").StateAwaitingForGloryKillInput += () => _killPrompt.Enable();
	}

	public TileData GetTileDataAtPosition(Vector2 position)
	{
		var tileMap = GetChildren().OfType<TileMap>().FirstOrDefault();
		var localPosition = tileMap.LocalToMap(position);

		return tileMap.GetCellTileData(0, localPosition);
	}
}
