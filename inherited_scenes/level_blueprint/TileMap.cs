using Godot;
using System;

public partial class TileMap : Godot.TileMap
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		UpdateSpikes();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	// Updates spikes collision on how far the ground is
	// Can be re-ran if the turrain ever changes
	//---- Spikes layer is 1, not a final choice -----
	private Godot.Collections.Array<Godot.Vector2I> _spikesCoordLst;
	private Godot.Collections.Array<Spike> _spikesLst = new();
	
	public void AddToLstSpike(Spike spike) {
		_spikesLst.Add(spike);
		UpdateSpikes();
	}
	public void UpdateSpikes(){
		_spikesCoordLst = GetUsedCells(1);
		foreach (var coord in _spikesCoordLst)
		{
			Vector2I coordCellBottom = GetNeighborCell(coord, TileSet.CellNeighbor.BottomSide); 
			while (true) { 
				var cell = GetCellTileData(0, coordCellBottom);
				if (cell != null)
				{
					int distance = Math.Abs(coordCellBottom.Y - coord.Y);

					Vector2 spikeLocalCoord = MapToLocal(coord);
					foreach (var spike in _spikesLst)
					{
						if (spike.GlobalPosition == spikeLocalCoord){
							spike.setRaycatDistance(distance);
							break;
						}
					}
					break;
				}
				coordCellBottom = GetNeighborCell(coordCellBottom, TileSet.CellNeighbor.BottomSide); 
			}
		}
	}
}
