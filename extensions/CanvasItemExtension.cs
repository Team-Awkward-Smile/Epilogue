using Godot;

namespace Epilogue.extensions;
public static class CanvasItemExtension
{
	/// <summary>
	///		Sets a parameter from the shader of this Material. This is shorthand for casting the Material to a ShaderMaterial
	/// </summary>
	/// <param name="parameterName">Name of the parameter</param>
	/// <param name="value">Value of the parameter</param>
	public static void SetShaderMaterialParameter(this CanvasItem item, StringName parameterName, Variant value)
	{
		var shaderMaterial = (ShaderMaterial) item.Material;

		shaderMaterial.SetShaderParameter(parameterName, value);
	}
}
