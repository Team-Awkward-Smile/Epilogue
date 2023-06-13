shader_type canvas_item;

uniform vec4 color : source_color;
uniform sampler2D SCREEN_TEXTURE : hint_screen_texture, filter_linear_mipmap;

void fragment() {
	vec4 original = texture(SCREEN_TEXTURE, SCREEN_UV);
	float mean = (original.r + original.g + original.b) / 3.0;
	vec4 grey = vec4(mean, mean, mean, original.a);
	
	if(original.r < color.r && original.g < color.g && original.b < color.b) {
		COLOR = mix(original, grey, 1.0);
	}
	else {
		COLOR = original;
	}
}
