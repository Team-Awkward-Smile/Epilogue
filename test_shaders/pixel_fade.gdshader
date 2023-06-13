shader_type canvas_item;

uniform sampler2D SCREEN_TEXTURE : hint_screen_texture, filter_linear_mipmap;
uniform float pixel_size : hint_range(0.0, 100.0);

void fragment() {
	vec2 new_uv = round(SCREEN_UV * pixel_size) / pixel_size;
	
	COLOR = texture(SCREEN_TEXTURE, new_uv);
}
