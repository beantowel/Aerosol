Shader "Aerosol/Skybox"
{
    Properties
    {
        transmittance_texture("Transimittance Texture", 2D) = "" {}
        scattering_texture("Scattering Texture", 3D) = "" {}
        single_mie_scattering_texture("Single Mie Scattering Texture", 3D) = "" {}
        irradiance_texture("Irradiance Texture", 2D) = "" {}
        unit_scale("Unit Scale", Float) = 0.001
        white_point("White Point", Vector) = (1, 1, 1, 0)
        exposure("Exposure", Float) = 0.0001
        ground_albedo("Ground Albedo", Vector) = (0.1, 0.1, 0.1, 0)
    }
    SubShader
    {
        Tags {"Queue"="Background" "RenderType"="Background"}
        Cull Off ZWrite Off ZTest Less

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_instancing

            #include "UnityCG.cginc"
            #include "atmosphere.hlsl"

            const static float3 earth_center = float3(0, -ATMOSPHERE.bottom_radius, 0);
            const static float2 sun_size = float2(
                tan(ATMOSPHERE.sun_angular_radius), cos(ATMOSPHERE.sun_angular_radius));

            float unit_scale;
            float3 white_point;
            float exposure;
            float3 ground_albedo;

            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float3 view_ray : TEXCOORD0;
            };

            v2f vert (appdata v)
            {
                v2f output;
                output.pos = UnityObjectToClipPos(v.vertex);
                output.view_ray = mul((float3x3)unity_ObjectToWorld, v.vertex.xyz);
                return output;
            }

            half4 frag (v2f i) : SV_Target
            {
                float3 view_direction = normalize(i.view_ray);
                float3 sun_direction = normalize(_WorldSpaceLightPos0.xyz);
                float3 camera = _WorldSpaceCameraPos * unit_scale;
                float3 p = camera - earth_center;
                float lightshaft_fadein_hack = smoothstep(
                    0.02, 0.04, dot(normalize(p), sun_direction));

                // TODO: shadow
                float shadow_out = 0;
                float shadow_in = 0;

                // Compute the radiance reflected by the ground, if the ray intersects it.
                // Compute the distance between the view ray line and the Earth center,
                // and the distance between the camera and the intersection of the view
                // ray with the ground (or NaN if there is no intersection).
                float ground_alpha = 0.0;
                float3 ground_luminance = 0.0;
                float p_dot_v = dot(p, view_direction);
                float p_dot_p = dot(p, p);
                float ray_earth_center_squared_distance = p_dot_p - p_dot_v * p_dot_v;
                float delta_intersection_square = earth_center.y * earth_center.y - ray_earth_center_squared_distance;
                float distance_to_intersection = -p_dot_v - sqrt(delta_intersection_square);
                if (delta_intersection_square > 0 && distance_to_intersection > 0) {
                    float3 pnt = p + view_direction * distance_to_intersection;
                    float3 normal = normalize(pnt);

                    // Compute the radiance reflected by the ground.
                    float3 sky_illuminance;
                    float3 sun_illuminance = GetSunAndSkyIlluminance(
                        pnt, normal, sun_direction, sky_illuminance);
                    // TODO: visibility term
                    float visibility = 1;
                    ground_luminance = ground_albedo * (1.0 / PI) *
                        (sun_illuminance + sky_illuminance) * visibility;

                    float shadow_length = 
                        max(0.0, min(shadow_out, distance_to_intersection) - shadow_in) *
                        lightshaft_fadein_hack;
                    float3 transmittance;
                    float3 in_scatter = GetSkyLuminanceToPoint(p, pnt,
                        shadow_length, sun_direction, transmittance);
                    ground_luminance = ground_luminance * transmittance + in_scatter;
                    ground_alpha = 1.0;
                }

                // Compute the radiance of the sky.
                float shadow_length = max(0, shadow_out - shadow_in) * lightshaft_fadein_hack;
                float3 transmittance;
                float3 luminance = GetSkyLuminance(
                    p, view_direction, shadow_length, sun_direction, transmittance);

                // If the view ray intersects the Sun, add the Sun radiance.
                if (dot(view_direction, sun_direction) > sun_size.y) {
                    luminance = luminance + transmittance * GetSolarLuminance();
                }
                luminance = lerp(luminance, ground_luminance, ground_alpha);

                // tone mapping & sRGB -> linearRGB (gamma)
                float3 rgb = pow(1 - exp(-luminance / white_point * exposure), 2.2);
                return half4(rgb , 1);
            }
            ENDHLSL
        }
    }
}
