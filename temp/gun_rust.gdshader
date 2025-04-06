shader_type canvas_item;

uniform bool highlightState = false;
uniform vec4 highlightColor : source_color;

uniform float dissolveState : hint_range(0, 1) = 0;
uniform sampler2D dissolveNoise;

void fragment() {
	if (highlightState)
	{
		float size_x = 15.0/float(textureSize(TEXTURE, 0).x);
		float size_y = 15.0/float(textureSize(TEXTURE, 0).y);
		vec4 sprite_color = texture(TEXTURE, UV);
		float alpha = -4.0 * sprite_color.a;
		alpha += texture(TEXTURE, UV + vec2(size_x, 0)).a;
		alpha += texture(TEXTURE, UV + vec2(-size_x, 0)).a;
		alpha += texture(TEXTURE, UV + vec2(0, size_y)).a;
		alpha += texture(TEXTURE, UV + vec2(0, -size_y)).a;
		vec4 final_color = mix(sprite_color, highlightColor, clamp(alpha, 0.0, 1.0));
		COLOR = vec4(final_color.rgb, clamp(abs(alpha) + sprite_color.a, 0.0, 1.0));
	}
	else
	{
		vec4 textureColor = texture(TEXTURE, UV);
		float brightness = texture(dissolveNoise, UV).r;
		
		COLOR = textureColor;
		COLOR.a *= step(dissolveState, brightness);
	}
	
}
