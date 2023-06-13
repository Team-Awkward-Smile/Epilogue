shader_type canvas_item;

uniform sampler2D SCREEN_TEXTURE : hint_screen_texture, filter_linear_mipmap;

void fragment() {
	vec2 new_uv = vec2(
		1.0 - SCREEN_UV.x,
		SCREEN_UV.y
	);
	
	COLOR = texture(SCREEN_TEXTURE, new_uv);
}
