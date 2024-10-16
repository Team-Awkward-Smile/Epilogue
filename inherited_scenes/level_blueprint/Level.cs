using Epilogue.Global.Enums;
using Epilogue.Global.Singletons;
using Epilogue.Props.camera;
using Epilogue.UI;
using Epilogue.UI.HP;
using Epilogue.UI.Pause;
using Godot;
using System.Collections.Generic;
using System.Linq;

namespace Epilogue.Nodes;
/// <summary>
///		Base Node for every level in the game (UIs are not Levels, keep that in mind)
/// </summary>
[GlobalClass, Icon("res://nodes/icons/level.png"), Tool]
public partial class Level : Node2D
{
	/// <summary>
	///		Reference to the player character
	/// </summary>
	public Player Player { get; set; }

	private PauseUI _pauseUI;
	private AmmoUI _ammoUI;
	private HPUI _hpUI;
	private Window _console;
	private GloryKillPrompt _killPrompt;
	private TileMap _tileMap;
	private PlayerEvents _playerEvents;
	private List<Checkpoint> _checkpoints = new();
	private Camera _camera;
	private CheckpointManager _checkpointManager;
	private AchievementPopup _achievementUI;

	/// <inheritdoc/>
	public override string[] _GetConfigurationWarnings()
	{
		var warnings = new List<string>();

		var checkpoints = GetNodeOrNull("Checkpoints");

		if (checkpoints is null || checkpoints.GetChildCount() == 0)
		{
			warnings.Add("This Level has no Checkpoints set.\nTo set a Checkpoint, add a Node2D called 'Checkpoints' as a child of this Level, and add the Checkpoints as children of it");
		}
		else if (!checkpoints.GetChildren().OfType<Checkpoint>().Where(c => c.FirstCheckpoint).Any())
		{
			warnings.Add("This Level has no default Checkpoint set.\nThe first Checkpoint found will be used");
		}
		else if (checkpoints.GetChildren().OfType<Checkpoint>().Where(c => c.FirstCheckpoint).Count() > 1)
		{
			var firstCheckpoints = checkpoints.GetChildren().OfType<Checkpoint>().Where(c => c.FirstCheckpoint);

			warnings.Add($"This Level has {firstCheckpoints.Count()} Checkpoint set as the First ({string.Join(", ", firstCheckpoints.Select(c => c.Name))}).\nOnly 1 Checkpoint should be set as the First");
		}

		return warnings.ToArray();
	}

	/// <inheritdoc/>
	public override void _Input(InputEvent @event)
	{
		if (@event.IsActionPressed("pause_game"))
		{
			_pauseUI.Enable();

			GetViewport().SetInputAsHandled();
		}
		else if (@event.IsActionPressed("console"))
		{
			_console.Visible = !_console.Visible;
		}
	}

	/// <inheritdoc/>
	public override void _EnterTree()
	{
		if (Engine.IsEditorHint())
		{
			return;
		}

		Player = GD.Load<PackedScene>("res://actors/hestmor/hestmor.tscn").Instantiate() as Player;

		AddChild(Player);
	}

	/// <inheritdoc/>
	public override void _Ready()
	{
		if (Engine.IsEditorHint())
		{
			return;
		}

		// TODO: 68 - Add them all to a List and Instantiate them all at once
		_pauseUI = GD.Load<PackedScene>("res://ui/pause/pause_ui.tscn").Instantiate() as PauseUI;
		_console = GD.Load<PackedScene>("res://ui/console.tscn").Instantiate() as Window;
		_killPrompt = GD.Load<PackedScene>("res://ui/glory_kill_prompt.tscn").Instantiate() as GloryKillPrompt;
		_ammoUI = GD.Load<PackedScene>("res://ui/ammo/ammo_ui.tscn").Instantiate() as AmmoUI;
		_hpUI = GD.Load<PackedScene>("res://ui/hp/hp_ui.tscn").Instantiate() as HPUI;
		_achievementUI = GD.Load<PackedScene>("res://ui/achievements/achievement_popup.tscn").Instantiate() as AchievementPopup;

		_playerEvents = GetNode<PlayerEvents>("/root/PlayerEvents");
		_checkpointManager = GetNode<CheckpointManager>("/root/CheckpointManager");

		_playerEvents.Connect(PlayerEvents.SignalName.PlayerDied, Callable.From(RespawnPlayer));
		_playerEvents.Connect(PlayerEvents.SignalName.QueryExecutionSpeed, Callable.From(_killPrompt.Enable));

		_tileMap = GetChildren().OfType<TileMap>().FirstOrDefault();

		// TODO: 68 - Maybe the root CanvasLayer should also be created at run-time?
		var uiLayer = GetNode<CanvasLayer>("UILayer");

		uiLayer.AddChild(_pauseUI);
		uiLayer.AddChild(_console);
		uiLayer.AddChild(_killPrompt);
		uiLayer.AddChild(_ammoUI);
		uiLayer.AddChild(_hpUI);
		uiLayer.AddChild(_achievementUI);

		_pauseUI.Hide();
		_console.Hide();
		_killPrompt.Disable();

		_console.Size = DisplayServer.WindowGetSize() / 3;

		ProcessMode = ProcessModeEnum.Pausable;

		var checkpointParent = GetNodeOrNull("Checkpoints");

		if (checkpointParent is not null)
		{
			var i = 0;

			// Setting every Checkpoint present in the Level
			foreach (var checkpoint in checkpointParent.GetChildren().OfType<Checkpoint>())
			{
				_checkpoints.Add(checkpoint);

				checkpoint.CheckpointTriggered += () => SetNewCheckpoint(checkpoint);
				checkpoint.ID = i++;

				// Marking the Checkpoint as Used if the player already touched it
				if (_checkpointManager.UsedCheckpointsIDs.Contains(checkpoint.ID))
				{
					checkpoint.Monitoring = false;
					checkpoint.SetCheckpointState(CheckpointState.Used);
				}
			}

			// Setting the current Checkpoint from the CheckpointManager singleton
			if (_checkpointManager.CurrentCheckpointID is not null)
			{
				_checkpoints.Where(c => c.ID == _checkpointManager.CurrentCheckpointID).First().Current = true;
			}
			else if (_checkpoints.Count > 0)
			{
				// If the singleton has no current Checkpoint, set the current one to the First
				_checkpoints.Where(c => c.FirstCheckpoint).First().Current = true;

				if (!_checkpoints.Any(c => c.Current))
				{
					_checkpoints.First().Current = true;
				}
			}
		}

		Player.Position = _checkpoints.FirstOrDefault(c => c.Current).Position;

		_camera = GetViewport().GetCamera2D() as Camera;
		_camera.Position = Player.Position;
		_camera.SetCameraTarget(Player.GetNode<Node2D>("CameraAnchor"));
	}

	/// <summary>
	/// 	Reloads the current scene after a small delay, and respawns the player character at the correct Checkpoint
	/// </summary>
	private void RespawnPlayer()
	{
		GetTree().CreateTimer(2f).Timeout += () =>
		{
			_playerEvents.PlayerDied -= RespawnPlayer;
			GetTree().ReloadCurrentScene();
		};
	}


	/// <summary>
	///		Sets the new current Checkpoint triggered by the player, deactivating the old one
	/// </summary>
	/// <param name="triggeredCheckpoint">New Checkpoint triggered</param>
	private void SetNewCheckpoint(Checkpoint triggeredCheckpoint)
	{
		var oldCheckpoint = _checkpoints.First(c => c.Current);

		oldCheckpoint.SetCheckpointState(CheckpointState.Used);
		oldCheckpoint.Current = false;

		var newCheckpoint = _checkpoints.First(c => c == triggeredCheckpoint);

		newCheckpoint.Current = true;
		newCheckpoint.SetDeferred("monitoring", false);
		newCheckpoint.SetCheckpointState(CheckpointState.Current);

		_checkpointManager.CurrentCheckpointID = newCheckpoint.ID;
		_checkpointManager.UsedCheckpointsIDs.Add(oldCheckpoint.ID);
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

		if (tileData is null)
		{
			return;
		}

		if (tileData.GetCustomData("destructible").AsBool())
		{
			var currentHp = tileData.GetCustomData("tile_hp").AsDouble() - damage;

			if (currentHp <= 0)
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
