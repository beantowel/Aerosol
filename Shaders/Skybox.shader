Shader "Aerosol/Skybox"
{
    Properties
    {
        transmittance_texture("Transimittance Texture", 2D) = "" {}
        scattering_texture("Scattering Texture", 3D) = "" {}
        single_mie_scattering_texture("Single Mie Scattering Texture", 3D) = "" {}
        irradiance_texture("Irradiance Texture", 2D) = "" {}
        unit_scale("Unit Scale", Float) = 0.001
        exposure("Exposure", Float) = 0.0001
        h_origin("Height Origin", Float) = 0
        ground_albedo("Ground Albedo", Vector) = (0.1, 0.1, 0.1, 1)
        white_point("White Point", Vector) = (1, 1, 1, 0)
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

            float4 ground_albedo;

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

            float3 getGroundLuminance(float r, float mu, float3 p, float3 viewDir, float3 sunDir)
            {
                float distance = DistanceToBottomAtmosphereBoundary(r, mu);
                float3 pnt = p + viewDir * distance;
                float3 normal = normalize(pnt);

                // Compute the radiance reflected by the ground.
                float3 sky_illuminance;
                float3 sun_illuminance = GetSunAndSkyIlluminance(
                    pnt, normal, sunDir, sky_illuminance);
                float3 ground_luminance = ground_albedo.xyz * ground_albedo.w * (1.0 / PI_ATOM) *
                    (sun_illuminance + sky_illuminance);

                float3 transmittance;
                float3 in_scatter = GetSkyLuminanceToPoint(p, pnt,
                    0, sunDir, transmittance);
                ground_luminance = ground_luminance * transmittance + in_scatter;
                return ground_luminance;
            }

            half4 frag (v2f i) : SV_Target
            {
                float3 viewDir = normalize(i.view_ray);
                float3 sunDir = normalize(_WorldSpaceLightPos0.xyz);
                float3 camera = (_WorldSpaceCameraPos - float3(0, h_origin, 0)) * unit_scale;
                float3 p = camera - earth_center;
                float r = length(p);
                float mu = dot(viewDir, float3(0, 1, 0));

                // Compute the radiance reflected by the ground, if the ray intersects it.
                // Compute the distance between the view ray line and the Earth center,
                // and the distance between the camera and the intersection of the view
                // ray with the ground (or NaN if there is no intersection).
                float3 luminance;
                if (RayIntersectsGround(r, mu)) {
                    luminance = getGroundLuminance(r, mu, p, viewDir, sunDir);
                } else {
                    // Compute the radiance of the sky.
                    float3 transmittance;
                    luminance = GetSkyLuminance(
                        p, viewDir, 0, sunDir, transmittance);
                    // If the view ray intersects the Sun, add the Sun radiance.
                    if (dot(viewDir, sunDir) > sun_size.y) {
                        luminance += transmittance * GetSolarLuminance();
                    }
                }

                // tone mapping (linearRGB)
                float3 rgb = 1 - exp(-luminance / white_point * exposure);
                return half4(rgb , 1);
            }
            ENDHLSL
        }
    }
}
