shader_type canvas_item;

uniform float dissolveState : hint_range(0, 1) = 0;
uniform sampler2D dissolveNoise;

void fragment() {
	vec4 textureColor = texture(TEXTURE, UV);
	float brightness = texture(dissolveNoise, UV).r;
	
	COLOR = textureColor;
	COLOR.a *= step(dissolveState, brightness);
}
