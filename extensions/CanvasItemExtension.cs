using Godot;

namespace Epilogue.extensions;
public static class CanvasItemExtension
{
	public static void SetShaderMaterialParameter(this CanvasItem item, StringName parameterName, Variant value)
	{
		var shaderMaterial = (ShaderMaterial) item.Material;

		shaderMaterial.SetShaderParameter(parameterName, value);
	}
}
