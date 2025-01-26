// NOTE: Shader automatically converted from Godot Engine 4.4.beta1.mono's StandardMaterial3D.
shader_type spatial;

uniform sampler2D mask:source_color;
uniform sampler2D grass_tex:source_color;
uniform sampler2D volcano_tex:source_color;
uniform sampler2D cliff_tex:source_color;
uniform sampler2D beach_tex:source_color;

uniform sampler2D black_normal:source_color;
uniform sampler2D red_normal:source_color;
uniform sampler2D green_normal:source_color;
uniform sampler2D blue_normal:source_color;

uniform sampler2D black_rough:hint_default_white;
uniform sampler2D red_rough:hint_default_white;
uniform sampler2D green_rough:hint_default_white;
uniform sampler2D blue_rough:hint_default_white;


uniform float uvsize:hint_range(0.0, 600, 0.1)=1;
uniform float uvsizeMount:hint_range(0.0, 200.0, 0.1)=24;


void fragment() {
	vec2 base_uv = UV*uvsize;
	vec2 base_uv_mount = UV*uvsizeMount;
	vec3 albedo_mask = texture(mask, UV).rgb;
	vec3 albedo_red = texture(beach_tex, base_uv).rgb;
	vec3 albedo_blue = texture(grass_tex, base_uv).rgb;
	vec3 albedo_green= texture(cliff_tex, base_uv).rgb;
	vec3 albedo_black = texture(volcano_tex, base_uv_mount).rgb;
	float sum_tex =(albedo_mask.r+albedo_mask.g+albedo_mask.b);
	vec3 mixed_tex = clamp((albedo_mask.r*albedo_red+ albedo_mask.g * albedo_green+ albedo_mask.b * albedo_blue)/sum_tex ,vec3(0.0),vec3(1.0));
	vec3 base_Normal_Tex= texture(black_normal,base_uv_mount).rgb;
	vec3 red_Normal_Tex= texture(red_normal,base_uv).rgb;
	vec3 blue_Normal_Tex= texture(blue_normal,base_uv).rgb;
	vec3 green_Normal_Tex= texture(green_normal,base_uv).rgb;
	vec3 mixedNormal = clamp((albedo_mask.r*red_Normal_Tex+ albedo_mask.g * green_Normal_Tex+ albedo_mask.b * blue_Normal_Tex)/sum_tex ,vec3(0.0),vec3(1.0));

	float total_weight = albedo_mask.r + albedo_mask.g + albedo_mask.b;
	vec3 weights = albedo_mask / total_weight;
	float base_rough_Tex = texture(black_rough,base_uv_mount).r ;
	float red_rough_Tex = texture(red_rough,base_uv).r * albedo_mask.r;
	float blue_rough_Tex = texture(blue_rough,base_uv).r *albedo_mask.b;
	float green_rough_Tex = texture(green_rough,base_uv).r*albedo_mask.g ;
	float mixedRough = clamp((weights.r*red_rough_Tex+ weights.g * green_rough_Tex+ weights.b * blue_rough_Tex)/sum_tex ,0,1);



	ALBEDO = mix(mixed_tex,albedo_black,1.0-sum_tex);
	NORMAL_MAP = mix(mixedNormal,base_Normal_Tex,1.0-sum_tex);
	ROUGHNESS = mix(mixedRough,base_rough_Tex,1.0-sum_tex);
	//ROUGHNESS = weights.r*red_rough_Tex+ weights.g * green_rough_Tex+ weights.b * blue_rough_Tex;


}
