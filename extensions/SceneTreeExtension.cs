using Epilogue.nodes;

using Godot;
using System;
using System.Linq;

public static class SceneTreeExtension
{
	public static Level GetLevel(this SceneTree sceneTree)
	{
		return sceneTree.Root.GetChildren().OfType<Level>().FirstOrDefault();
	}
}
