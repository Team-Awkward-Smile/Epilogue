#if TOOLS
using Epilogue.Constants;

using Godot;

namespace Epilogue.addons;
[Tool]
public partial class Plugin : EditorPlugin
{
	private NavigationTrigger _navigationTrigger;
	private SelectedTriggerPoint? _selectedPoint;

	public override void _Edit(GodotObject @object)
	{
		if(@object is NavigationTrigger navigationTrigger)
		{
			_navigationTrigger = navigationTrigger;
			_navigationTrigger.Selected = true;
			_navigationTrigger.QueueRedraw();
		}
		else
		{
			_navigationTrigger.Selected = false;
			_navigationTrigger.QueueRedraw();
			_navigationTrigger = null;
		}

		UpdateOverlays();
	}

	public override bool _Handles(GodotObject @object)
	{
		return @object is NavigationTrigger;
	}

	private void SelectTrigger()
	{
		var selectionSingleton = GetEditorInterface().GetSelection();

		selectionSingleton.Clear();
		selectionSingleton.AddNode(_navigationTrigger);
	}

	public override void _ForwardCanvasDrawOverViewport(Control viewportControl)
	{
		if(_navigationTrigger is null)
		{
			return;
		}

		var viewportTransform = _navigationTrigger.GetViewportTransform();
		var zoom = viewportTransform[0].X;

		var usedRect = _navigationTrigger.TileMap.GetUsedRect();
		var upperLeftCorner = _navigationTrigger.ToGlobal(_navigationTrigger.TileMap.MapToLocal(usedRect.Position - new Vector2I(5, 5)));
		var lowerRightCorner = _navigationTrigger.ToGlobal(_navigationTrigger.TileMap.MapToLocal(usedRect.End + new Vector2I(5, 5)));

		var startingPoint = new Vector2(
			viewportTransform[2].X + (upperLeftCorner.X * zoom),
			viewportTransform[2].Y + (upperLeftCorner.Y * zoom)
		);

		var endingPoint = new Vector2(
			viewportTransform[2].X + (lowerRightCorner.X * zoom),
			viewportTransform[2].Y + (lowerRightCorner.Y * zoom)
		);

		var step = Constants.Constants.MAP_TILE_SIZE * zoom;
		var halfTileOffset = zoom * Constants.Constants.MAP_TILE_SIZE / 2;

		var tilesX = (usedRect.Size.X + 11) * 2;
		var tilesY = (usedRect.Size.Y + 11) * 2;

		var pointsX = new Vector2[tilesX];
		var pointsY = new Vector2[tilesY];

		for(var i = 0; i < tilesX; i++)
		{
			pointsX[i] = new Vector2(
				startingPoint.X - halfTileOffset + (i / 2 * step),
				startingPoint.Y - halfTileOffset + ((endingPoint.Y - startingPoint.Y) * (i % 2))
			);

			if(i == 1 || i == tilesX - 1)
			{
				viewportControl.DrawLine(pointsX[i - 1] - new Vector2(0f, halfTileOffset * 2), pointsX[i] + new Vector2(0f, halfTileOffset * 2), new Color(0.44f, 0.65f, 0.92f, 0.3f));
			}
		}

		for(var i = 0; i < tilesY; i++)
		{
			pointsY[i] = new Vector2(
				startingPoint.X - halfTileOffset + ((endingPoint.X - startingPoint.X) * (i % 2)),
				startingPoint.Y - halfTileOffset + (i / 2 * step)
			);

			if(i == 1 || i == tilesY - 1)
			{
				viewportControl.DrawLine(pointsY[i - 1] - new Vector2(halfTileOffset * 2, 0f), pointsY[i] + new Vector2(halfTileOffset * 2, 0f), new Color(0.44f, 0.65f, 0.92f, 0.3f));
			}
		}

		viewportControl.DrawMultiline(pointsX[2..^2], new Color(0.44f, 0.65f, 0.92f));
		viewportControl.DrawMultiline(pointsY[2..^2], new Color(0.44f, 0.65f, 0.92f));
	}

	public override bool _ForwardCanvasGuiInput(InputEvent @event)
	{
		if(@event is InputEventMouseButton mouseClick && mouseClick.ButtonIndex == MouseButton.Left && _navigationTrigger is not null)
		{
			if(_navigationTrigger.StartPoint.DistanceTo(_navigationTrigger.GetLocalMousePosition()) <= 10f)
			{
				_selectedPoint = SelectedTriggerPoint.StartPoint;
				return true;
			}
			else if(_navigationTrigger.EndPoint.DistanceTo(_navigationTrigger.GetLocalMousePosition()) <= 10f)
			{
				_selectedPoint = SelectedTriggerPoint.EndPoint;
				return true;
			}

			_selectedPoint = null;

			return false;

		}
		else if(@event is InputEventMouseMotion && Input.IsMouseButtonPressed(MouseButton.Left) && _navigationTrigger is not null)
		{
			var gridCell = _navigationTrigger.TileMap.LocalToMap(_navigationTrigger.GetGlobalMousePosition());
			var cellGlobalPosition = _navigationTrigger.TileMap.MapToLocal(gridCell);

			if(_selectedPoint == SelectedTriggerPoint.StartPoint)
			{
				_navigationTrigger.StartPoint = _navigationTrigger.ToLocal(cellGlobalPosition);
				return true;
			}
			else if(_selectedPoint == SelectedTriggerPoint.EndPoint)
			{
				_navigationTrigger.EndPoint = _navigationTrigger.ToLocal(cellGlobalPosition);
				return true;
			}
		}

		return false;
	}

	private enum SelectedTriggerPoint
	{
		StartPoint,
		EndPoint
	}
}
#endif
