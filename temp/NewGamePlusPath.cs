using Epilogue.global.singletons;
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
		if(Settings.GameCycle == Epilogue.global.enums.GameCycle.NewGamePlus)
		{
			QueueFree();
		}
	}
}
