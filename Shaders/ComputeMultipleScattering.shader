Shader "Aerosol/ComputeMultipleScattering"
{
    Properties
    {
    }

    SubShader
    {
        Pass
        {
            Cull Off ZWrite Off ZTest Always
            Blend 0 Off
            Blend 1 One One
            HLSLPROGRAM
            #pragma vertex vertex
            #pragma fragment frag

            #include "UnityCG.cginc"
            #include "functions.hlsl"

            sampler2D transmittance_texture;
            sampler3D scattering_density_texture;
            float4x4 luminance_from_radiance;
            int layer;

            struct VS_INPUT
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct VS_OUTPUT
            {
                float4 pos : SV_POSITION;
                float2 texcoords : TEXCOORD0;
            };

            struct PS_OUTPUT
            {
                float3 delta_multiple_scattering : SV_Target0;
                float4 scattering : SV_Target1;
            };

            VS_OUTPUT vertex(VS_INPUT v)
            {
                VS_OUTPUT output;
                output.pos = UnityObjectToClipPos(v.vertex);
                output.texcoords = v.uv * SCATTERING_TEXTURE_SIZE.xy;
                return output;
            }

            PS_OUTPUT frag(VS_OUTPUT input)
            {
                PS_OUTPUT output;
                float nu;
                output.delta_multiple_scattering = ComputeMultipleScatteringTexture(
                    ATMOSPHERE, transmittance_texture, scattering_density_texture,
                    float3(input.texcoords, layer + 0.5), nu);
                output.scattering = float4(
                    mul((float3x3)luminance_from_radiance, output.delta_multiple_scattering) /
                        RayleighPhaseFunction(nu),
                    0.0);
                return output;
            }
            ENDHLSL
        }
    }
}
