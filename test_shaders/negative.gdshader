shader_type canvas_item;

uniform sampler2D SCREEN_TEXTURE : hint_screen_texture, filter_linear_mipmap;

void fragment() {
	vec4 original = texture(SCREEN_TEXTURE, SCREEN_UV);
	vec4 negative = vec4(
		1.0 - original.r,
		1.0 - original.g,
		1.0 - original.b,
		original.a	
	);
	
	COLOR = negative.rgba;
}
