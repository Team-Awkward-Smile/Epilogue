shader_type canvas_item;

uniform bool iframeActive = false;

void fragment() {
	if(iframeActive)
	{
		float timer = sin(TIME * 30.0);
		
		if(timer >= 0.0)
		{
			COLOR = vec4(1.0, 1.0, 1.0, COLOR.a);
		}
	}
}
