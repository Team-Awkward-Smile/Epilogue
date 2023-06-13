shader_type canvas_item;

uniform float force : hint_range(0.0, 1.0, 0.1);
uniform sampler2D SCREEN_TEXTURE : hint_screen_texture, filter_linear_mipmap;

void fragment() {
	vec4 original = texture(SCREEN_TEXTURE, SCREEN_UV);
	float mean = (original.r + original.g + original.b) / 3.0;
	vec4 grey = vec4(mean, mean, mean, original.a);
	
	COLOR = mix(original, grey, force);
}
