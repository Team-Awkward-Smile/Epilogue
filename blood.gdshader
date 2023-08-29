shader_type canvas_item;

uniform sampler2D bloodTexture;
uniform float bloodAmmount : hint_range(0.0, 1.0) = 0.0;

void fragment() {
	float bAmmount = sin(TIME / 4.0);
	
	vec2 spriteUvSize = vec2(48, 60) * TEXTURE_PIXEL_SIZE;
	vec2 translatedUv = mod(UV, spriteUvSize);
	vec2 newUv = translatedUv / spriteUvSize;
	vec4 textureColor = texture(TEXTURE, UV);
	vec4 noiseTexture = texture(bloodTexture, newUv);
	float brightness = texture(bloodTexture, newUv).r;
	vec4 color = COLOR;
	
	if(COLOR.a != 0.0 && step(brightness, bAmmount) == 1.0)
	{
		color = vec4(0.8, 0.0, 0.0, 1.0);
	}
	
	COLOR = color;
}
