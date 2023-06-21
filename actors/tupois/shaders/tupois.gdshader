shader_type canvas_item;

uniform float force = 3.0;
uniform vec2 center;
uniform sampler2D SCREEN_TEXTURE : hint_screen_texture, filter_linear_mipmap;

void fragment() {
	vec4 original = texture(TEXTURE, UV);
	
	if(original.a != 0.0)
	{
		vec2 distortion = normalize(SCREEN_UV - center) * force;
		COLOR  = texture(SCREEN_TEXTURE, SCREEN_UV - distortion);
	}
}
