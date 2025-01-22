shader_type canvas_item;

uniform bool iFrameActive = false;
uniform vec4 blinkColor : source_color = vec4(1.0);

void fragment() {
	if(iFrameActive)
	{
		float timer = sin(TIME * 30.0);
		
		if(timer >= 0.0)
		{
			COLOR = vec4(blinkColor.rgb, COLOR.a);
		}
	}
}
