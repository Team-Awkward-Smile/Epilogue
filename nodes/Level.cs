using Epilogue.global.singletons;
using Epilogue.ui;
using Godot;
using System.Linq;

namespace Epilogue.nodes;
[GlobalClass]
public partial class Level : Node2D
{
	private PauseUI _pauseUI;
	private AmmoUI _ammoUI;
	private Window _console;
	private GloryKillPrompt _killPrompt;
	private TileMap _tileMap;

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
		// TODO: 68 - Add them all to a List and Instantiate them all at once
		_pauseUI = GD.Load<PackedScene>("res://ui/pause_ui.tscn").Instantiate() as PauseUI;
		_console = GD.Load<PackedScene>("res://ui/console.tscn").Instantiate() as Window;
		_killPrompt = GD.Load<PackedScene>("res://ui/glory_kill_prompt.tscn").Instantiate() as GloryKillPrompt;
		_ammoUI = GD.Load<PackedScene>("res://ui/ammo_ui.tscn").Instantiate() as AmmoUI;

		// TODO: 68 - Maybe the root CanvasLayer should also be created at run-time?
		var uiLayer = GetNode<CanvasLayer>("UILayer");

		uiLayer.AddChild(_pauseUI);
		uiLayer.AddChild(_console);
		uiLayer.AddChild(_killPrompt);
		uiLayer.AddChild(_ammoUI);

		_pauseUI.Hide();
		_console.Hide();
		_killPrompt.Disable();

		_console.Size = DisplayServer.WindowGetSize() / 3;

		ProcessMode = ProcessModeEnum.Pausable;

		GetNode<Events>("/root/Events").StateAwaitingForGloryKillInput += () => _killPrompt.Enable();

		_tileMap = GetChildren().OfType<TileMap>().FirstOrDefault();
	}

	public TileData GetTileDataAtPosition(Vector2 position)
	{
		var localPosition = _tileMap.LocalToMap(position);

		return _tileMap.GetCellTileData(0, localPosition);
	}

	public void DamageTile(Vector2 tilePosition, float damage)
	{
		var localPosition = _tileMap.LocalToMap(tilePosition);
		var tileData = _tileMap.GetCellTileData(0, localPosition);

		if(tileData is null)
		{
			return;
		}

		if(tileData.GetCustomData("destructible").AsBool())
		{
			var currentHp = tileData.GetCustomData("tile_hp").AsDouble() - damage;

			if(currentHp <= 0)
			{
				_tileMap.EraseCell(0, localPosition);
			}
			else
			{
				tileData.SetCustomData("tile_hp", currentHp);
			}
		}
	}
}
