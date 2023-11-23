using Epilogue.nodes;
using Godot;
using System.Linq;

namespace Epilogue.extensions;
/// <summary>
/// 	Helper methods for the SceneTree
/// </summary>
public static class SceneTreeExtension
{
	/// <summary>
	/// 	Gets a reference to the currently loaded Level
	/// </summary>
	/// <param name="sceneTree">The current SceneTree</param>
	/// <returns>A reference to the current Level</returns>
	public static Level GetLevel(this SceneTree sceneTree)
	{
		return sceneTree.Root.GetChildren().OfType<Level>().FirstOrDefault();
	}
}
