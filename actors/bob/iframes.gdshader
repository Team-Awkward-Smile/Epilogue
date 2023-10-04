shader_type canvas_item;

uniform bool iframeActive = false;
uniform bool holdingGun = false;

void fragment() {
	if(iframeActive)
	{
		float timer = sin(TIME * 30.0);
		
		if(timer >= 0.0)
		{
			COLOR = vec4(1.0, 1.0, 1.0, COLOR.a);
		}
	}
	
	if(holdingGun)
	{
		if(COLOR == vec4(63.0 / 255.0, 63.0 / 255.0, 116.0 / 255.0, 1.0))
		{
			COLOR = vec4(COLOR.rgb, 0.0);
		}
	}
}
