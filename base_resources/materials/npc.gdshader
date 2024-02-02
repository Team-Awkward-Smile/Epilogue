shader_type canvas_item;

uniform bool iFrameActive = false;
uniform bool vulnerabilityActive = false;

void fragment() {
	if(vulnerabilityActive)
	{
		float wave = sin(TIME * 30.0);
		vec4 color = wave > 0.0 ? vec4(COLOR.r, COLOR.g * 0.6, COLOR.b * 0.2, COLOR.a) : COLOR.rgba;
		
		COLOR = color;
	}
	else if(iFrameActive)
	{
		float offset = sin(TIME * 30.0) > 0.0 ? 0.0 : 1.0;
		
		COLOR = vec4(COLOR.r + offset, COLOR.g + offset, COLOR.b + offset, COLOR.a);
	}
}