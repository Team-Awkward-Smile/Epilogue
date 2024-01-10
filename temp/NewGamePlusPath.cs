using Epilogue.Global.Singletons;
using Godot;

namespace Epilogue.temp;
/// <summary>
/// 	Temporary Node used to open up new paths during New Game+
/// </summary>
public partial class NewGamePlusPath : TileMap
{
	/// <inheritdoc/>
	public override void _Ready()
	{
		if(Settings.GameCycle == Epilogue.Global.Enums.GameCycle.NewGamePlus)
		{
			QueueFree();
		}
	}
}
