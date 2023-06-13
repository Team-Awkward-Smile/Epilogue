shader_type canvas_item;

uniform float force = 3.0;
uniform sampler2D SCREEN_TEXTURE : hint_screen_texture, filter_linear_mipmap;
uniform vec2 center;

void fragment() {
	float ratio = SCREEN_PIXEL_SIZE.x / SCREEN_PIXEL_SIZE.y;
	vec2 scaledUV = (SCREEN_UV - vec2(0.5, 0.0)) / vec2(ratio, 1.0) + vec2(0.5, 0.0);
	vec2 distortion = normalize(scaledUV - center) * force;
	
	COLOR = texture(TEXTURE, SCREEN_UV - distortion);
}
