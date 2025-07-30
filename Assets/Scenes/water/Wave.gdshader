shader_type spatial;
render_mode diffuse_lambert;

uniform float uvscale;
uniform vec3 water_clor:source_color;
uniform vec3 fresnl:source_color;
uniform float metalic:hint_range(0.0, 1.0)=0;
uniform float roughness:hint_range(0.0, 1.0)=0.02;
uniform sampler2D texture_normal;
uniform sampler2D texture_normal2;
uniform vec2 wave_direction =vec2(2.0,0);
uniform vec2 wave_direction2=vec2(0.0,1.0);
uniform float time_scale :hint_range(0.0,0.2, 0.005)= 0.025;

float fresnelpow(float amount, vec3 normal, vec3 view){
	return pow((1.0-clamp(dot(normalize(normal),normalize(view)),0.0,1.0)),amount);
}

void vertex() {

}

void fragment() {
	// Called for every pixel the material is visible on.
	vec2 uvscaled = UV*uvscale;
	vec2 time =(TIME *wave_direction)*time_scale;
	vec2 time2 =(TIME *wave_direction2)*time_scale;
	vec3 normal =mix(texture(texture_normal,uvscaled+time).rgb,texture(texture_normal2,uvscaled+time2).rgb,0.5);
	float frensel = fresnelpow(3.0,NORMAL,VIEW);
	vec3 water = mix(water_clor,fresnl,frensel);
	ALBEDO = water;
	METALLIC = metalic;
	ROUGHNESS=roughness;
	NORMAL_MAP = normal;
}

void light() {

	vec3 N = NORMAL;
	vec3 L = LIGHT;
	vec3 diffusecolor=ALBEDO;
	vec3 lightcolor = LIGHT_COLOR;
	float ndotl = dot(N,L);
	float cndotl = max(0.0,ndotl)*(1.0/PI);

	vec3 diffuselight = cndotl*diffusecolor*lightcolor*ATTENUATION;

 	DIFFUSE_LIGHT += diffuselight;

}
