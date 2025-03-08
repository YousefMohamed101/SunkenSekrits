shader_type spatial;
render_mode diffuse_lambert,shadows_disabled;

uniform vec3 water_clor:source_color;
uniform vec3 fresnl:source_color;
uniform float metalic:hint_range(0.0, 1.0)=0;
uniform float roughness:hint_range(0.0, 1.0)=0.02;
uniform sampler2D texture_normal;
uniform sampler2D texture_normal2;
uniform vec2 wave_direction =vec2(2.0,0);
uniform vec2 wave_direction2=vec2(0.0,1.0);
uniform float time_scale :hint_range(0.0,0.2, 0.005)= 0.025;

uniform vec4 WaveA;
uniform vec4 WaveB;
uniform vec4 WaveC;
uniform vec4 WaveD;
vec3 GerstnerWave(vec4 wave,vec3 coord , inout vec3 tangent,inout vec3 binormal){
	float steep = wave.z;
	float frequency = wave.a;
	float k = 2.0*PI /frequency;
	float c = sqrt(9.8/k);
	vec2 d =normalize(wave.xy);
	float a = steep/k;
	float f = k*(dot(d,coord.xz)-TIME*c);

	tangent += vec3(1.0-d.x*d.x*(steep*sin(f)),d.x*(steep*cos(f)),-d.x*d.y*(steep*sin(f)));
	//vec3 tange =normalize(vec3(1,frequency*amplitude*cos(f),0));
	binormal += vec3(-d.x*d.y*(steep*sin(f)),d.y*(steep*cos(f)),1.0-d.y*d.y*(steep*sin(f)));

	return vec3(d.x*(a*cos(f)),a*sin(f),d.y*(a*cos(f)));
}

float fresnelpow(float amount, vec3 normal, vec3 view){
	return pow((1.0-clamp(dot(normalize(normal),normalize(view)),0.0,1.0)),amount);
}

void vertex() {
	TANGENT =vec3(0.0);
	BINORMAL= vec3(0.0);

	VERTEX += GerstnerWave(WaveA,VERTEX,TANGENT,BINORMAL);
	VERTEX += GerstnerWave(WaveB,VERTEX,TANGENT,BINORMAL);
	VERTEX += GerstnerWave(WaveC,VERTEX,TANGENT,BINORMAL);
	vec3 normal = normalize(cross(BINORMAL,TANGENT));
	NORMAL = normal;
}

void fragment() {
	// Called for every pixel the material is visible on.
	vec2 time =(TIME *wave_direction)*time_scale;
	vec2 time2 =(TIME *wave_direction2)*time_scale;
	vec3 normal =mix(texture(texture_normal,UV+time).rgb,texture(texture_normal2,UV+time2).rgb,0.5);
	float frensel = fresnelpow(5.0,NORMAL,VIEW);
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
