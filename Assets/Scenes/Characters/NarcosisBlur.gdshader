shader_type canvas_item;

uniform sampler2D screen_texture : hint_screen_texture, repeat_disable, filter_nearest;
uniform float blur_strength : hint_range(0.0,4.0) =0.0;

const float totalGausianWeight =273.0;
const float weights[25] = float[](
        1.0, 4.0, 6.0, 4.0, 1.0,
        4.0, 16.0, 24.0, 16.0, 4.0,
        6.0, 24.0, 36.0, 24.0, 6.0,
        4.0, 16.0, 24.0, 16.0, 4.0,
        1.0, 4.0, 6.0, 4.0, 1.0
    );

vec4 gaussian_blur(sampler2D tex, vec2 uv, vec2 pixel_size, float strength) {
    vec4 color = vec4(0.0);
    float total_weight = 0.0;



    int index = 0;
    for (int x = -2; x <= 2; x++) {
        for (int y = -2; y <= 2; y++) {
            vec2 offset = vec2(float(x), float(y)) * pixel_size * strength;
            color += texture(tex, uv + offset) * weights[index];
            index++;
        }
    }

    return color / totalGausianWeight;
}

void fragment() {
	vec4 color = vec4(0.0);
    float total_weight = 0.0;
	vec2 uv = UV;


    int index = 0;
    for (int x = -2; x <= 2; x++) {
        for (int y = -2; y <= 2; y++) {
            vec2 offset = vec2(float(x), float(y)) * SCREEN_PIXEL_SIZE * blur_strength;
            color += texture(screen_texture, SCREEN_UV + vec2(float(x), float(y))* blur_strength* SCREEN_PIXEL_SIZE) * weights[index];
            index++;
        }
    }
	COLOR = color/ totalGausianWeight;
    //COLOR = gaussian_blur(screen_texture, SCREEN_UV, SCREEN_PIXEL_SIZE, blur_strength);
}