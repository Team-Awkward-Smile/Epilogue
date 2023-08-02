using Epilogue.global.singletons;
using Epilogue.ui;
using Godot;
using System.Linq;

namespace Epilogue.nodes;
/// <summary>
///		Base Node for every level in the game (UIs are not Levels, keep that in mind)
/// </summary>
[GlobalClass, Icon("res://nodes/icons/level.png")]
public partial class Level : Node2D
{
	private PauseUI _pauseUI;
	private AmmoUI _ammoUI;
	private Window _console;
	private GloryKillPrompt _killPrompt;
	private TileMap _tileMap;

	/// <inheritdoc/>
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

	/// <inheritdoc/>
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

		GetNode<PlayerEvents>("/root/PlayerEvents").StateAwaitingForExecutionSpeed += () => _killPrompt.Enable();

		_tileMap = GetChildren().OfType<TileMap>().FirstOrDefault();
	}

	/// <summary>
	///		Gets the TileData of the Tile at the selected position
	/// </summary>
	/// <param name="position">Position of the desired tile, in GridMap units</param>
	/// <returns>The TileData of the corresponding Tile, or null if no Tiles are present at that position</returns>
	public TileData GetTileDataAtPosition(Vector2 position)
	{
		var localPosition = _tileMap.LocalToMap(position);

		return _tileMap.GetCellTileData(0, localPosition);
	}

	/// <summary>
	///		Deals damage to a Tile at the selected position
	/// </summary>
	/// <param name="tilePosition">Position of the Tile, in GridMap units</param>
	/// <param name="damage">Damage to be dealt</param>
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
