shader_type canvas_item;

// Three independent inputs: session mapping/sight, spatial exposure, and live Light2D illumination.
uniform sampler2D geography : filter_nearest, repeat_disable;
uniform sampler2D exposure : filter_linear, repeat_disable;
uniform vec2 world_size = vec2(640.0, 352.0);
uniform vec4 ground_rect = vec4(0.0);
uniform int surface_kind = 0; // 0: terrain, 1: remembered structure, 2: current dynamic object
uniform bool discovery_enabled = false;
uniform bool neutral = false;
uniform bool contact_shadow = false;
uniform vec4 cave_ambient : source_color = vec4(0.78, 0.80, 0.84, 1.0);
uniform vec4 sky_ambient : source_color = vec4(1.0, 0.98, 0.94, 1.0);
varying vec2 world_pixel;
varying vec4 tint;

// Distance to an unrevealed neighbour, evaluated ONLY inside a logically revealed cell.
// Feathering erodes visible presentation; it never expands authoritative sight or mapping.
float coverage(vec2 p, int channel) {
    vec2 cell=floor(p/16.0);
    float distance_to_dark=24.0;
    for(int y=-1;y<=1;y++) for(int x=-1;x<=1;x++) {
        vec2 q=cell+vec2(float(x),float(y));
        vec3 k=texture(geography,(q*16.0+vec2(8.0))/world_size).rgb;
        float known=channel==0?k.r:k.g;
        if(known<0.5){vec2 lo=q*16.0;vec2 d=max(max(lo-p,p-lo-vec2(16.0)),vec2(0.0));distance_to_dark=min(distance_to_dark,length(d));}
    }
    float feather=7.0+sin(p.x*0.017+sin(p.y*0.023))*1.5;
    return smoothstep(0.0,feather,distance_to_dark);
}

void vertex() {
    world_pixel = (MODEL_MATRIX * vec4(VERTEX, 0.0, 1.0)).xy;
    tint = COLOR;
}
void fragment() {
    vec4 albedo = texture(TEXTURE, UV) * tint;
    vec2 sample_point = world_pixel;
    if (surface_kind > 0) {
        // Lifted artwork belongs to ground beneath it, not the wall/unknown cell behind its canopy.
        sample_point = clamp(world_pixel, ground_rect.xy + vec2(0.1), ground_rect.xy + ground_rect.zw - vec2(0.1));
    }
    vec3 knowledge = texture(geography, sample_point / world_size).rgb;
    float sky = texture(exposure, sample_point / world_size).r;
    vec3 ambient = neutral ? vec3(1.0) : mix(cave_ambient.rgb, sky_ambient.rgb, sky);
    // Roofless interiors retain diffuse sky exposure without receiving cave ambience.
    float sheltered = texture(exposure, sample_point / world_size).g;
    ambient *= neutral ? 1.0 : mix(1.0, 0.88, sheltered);
    if(contact_shadow)ambient=vec3(1.0);
    if (discovery_enabled) {
        if (surface_kind == 2 && (knowledge.g < 0.5 || knowledge.b > 0.5)) { discard; }
        if (surface_kind == 1 && knowledge.r < 0.5) { discard; }
        if (surface_kind == 0 && knowledge.r < 0.5) { COLOR = vec4(0.063, 0.098, 0.118, 1.0); }
        else {
            float memory = knowledge.g < 0.5 ? 0.48 : mix(0.48,1.0,coverage(sample_point,1));
            vec3 fogged=mix(vec3(0.063,0.098,0.118),albedo.rgb*ambient*memory,coverage(sample_point,0));
            COLOR = vec4(fogged, albedo.a*(surface_kind==2?coverage(sample_point,1):1.0));
        }
    } else { COLOR = vec4(albedo.rgb * ambient, albedo.a); }
}
void light() {
    // Unknown terrain is a map boundary, never illuminated evidence of hidden geography.
    vec2 p = surface_kind > 0 ? clamp(world_pixel, ground_rect.xy + vec2(0.1), ground_rect.xy + ground_rect.zw - vec2(0.1)) : world_pixel;
    vec3 k = texture(geography, p / world_size).rgb;
    LIGHT = contact_shadow || (discovery_enabled && k.g < 0.5) ? vec4(0.0) : vec4(COLOR.rgb * LIGHT_COLOR.rgb * LIGHT_ENERGY, LIGHT_COLOR.a);
}
